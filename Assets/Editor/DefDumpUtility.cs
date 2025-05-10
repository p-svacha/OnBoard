// Assets/Editor/DefDumpUtility.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class DefDumpUtility
{
    // keyed by "ClassName.PropertyName" and "ClassName.__class"
    private static Dictionary<string, string> _summaries;

    [MenuItem("Tools/Dump All Defs to Text…")]
    public static void DumpAllDefs()
    {
        DefDatabaseRegistry.ClearAllDatabases();
        DefDatabaseRegistry.AddAllDefs();
        DefDatabaseRegistry.ResolveAllReferences();
        DefDatabaseRegistry.OnLoadingDone();

        _summaries = BuildSourceSummaries();

        string path = EditorUtility.SaveFilePanel(
            "Save Def Dump", "", "AllDefsDump.txt", "txt"
        );
        if (string.IsNullOrEmpty(path)) return;

        using var writer = new StreamWriter(path, false, Encoding.UTF8);

        writer.WriteLine("=== DEF CLASSES ===\n");
        DumpDefTypes(writer);

        writer.WriteLine("\n=== STATIC XyzDefs LISTS ===\n");
        DumpStaticDefsLists(writer);

        AssetDatabase.Refresh();
        Debug.Log($"Def dump written to: {path}");
    }

    private static Dictionary<string, string> BuildSourceSummaries()
    {
        var dict = new Dictionary<string, string>();
        var guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (!path.EndsWith(".cs")) continue;
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].TrimStart().StartsWith("/// <summary>")) continue;

                var sb = new StringBuilder();
                i++;
                while (i < lines.Length && !lines[i].TrimStart().StartsWith("/// </summary>"))
                {
                    var t = lines[i].Trim();
                    if (t.StartsWith("///")) t = t.Substring(3).Trim();
                    sb.AppendLine(t);
                    i++;
                }
                var summary = sb.ToString().Replace("\r", "").Replace("\n", " ").Trim();

                // look ahead
                int j = i + 1;
                for (; j < lines.Length; j++)
                {
                    var line = lines[j].Trim();
                    // property?
                    var pm = Regex.Match(line, @"public\s+[^\s]+\s+(?<prop>\w+)\s*\{");
                    if (pm.Success)
                    {
                        string className = null;
                        for (int k = j; k >= 0; k--)
                        {
                            var cm = Regex.Match(lines[k], @"class\s+(?<type>\w+)");
                            if (cm.Success)
                            {
                                className = cm.Groups["type"].Value;
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(className))
                        {
                            var key = $"{className}.{pm.Groups["prop"].Value}";
                            if (!dict.ContainsKey(key))
                                dict[key] = summary;
                        }
                        break;
                    }
                    // class?
                    var cm2 = Regex.Match(line, @"class\s+(?<type>\w+)");
                    if (cm2.Success)
                    {
                        var className = cm2.Groups["type"].Value;
                        var key = $"{className}.__class";
                        if (!dict.ContainsKey(key))
                            dict[key] = summary;
                        break;
                    }
                }
            }
        }
        return dict;
    }

    private static string GetSummary(Type t, string member)
    {
        var key = $"{t.Name}.{member}";
        return _summaries != null && _summaries.TryGetValue(key, out var s) ? s : "";
    }

    private static void DumpDefTypes(StreamWriter writer)
    {
        var defTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Def).IsAssignableFrom(t))
            .OrderBy(t => t.Name);

        foreach (var defType in defTypes)
        {
            writer.WriteLine($"Class: {defType.Name}");
            var clsSum = GetSummary(defType, "__class");
            if (!string.IsNullOrEmpty(clsSum))
                writer.WriteLine($"  Description: {clsSum}");
            writer.WriteLine();

            var props = defType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic)
                .ToList();

            writer.WriteLine("  Properties:");
            foreach (var p in props)
            {
                var sum = GetSummary(defType, p.Name);
                writer.WriteLine($"    {p.PropertyType.Name} {p.Name}" + (sum == "" ? "" : $": {sum}"));
            }
            writer.WriteLine();

            writer.WriteLine("  Instances:");
            var dbType = typeof(DefDatabase<>).MakeGenericType(defType);
            var allDefsProp = dbType.GetProperty("AllDefs", BindingFlags.Public | BindingFlags.Static);
            var allDefs = allDefsProp?.GetValue(null) as IEnumerable;
            if (allDefs == null)
            {
                writer.WriteLine("    (no instances)\n");
                continue;
            }

            foreach (var def in allDefs)
            {
                object defName = "", label = "";
                try
                {
                    defName = defType.GetProperty("DefName").GetValue(def);
                    label = defType.GetProperty("LabelCap").GetValue(def);
                }
                catch { /* skip name/label if broken */ }

                writer.WriteLine($"    - {defName} ({label})");
                foreach (var p in props)
                {
                    try
                    {
                        var val = p.GetValue(def);
                        writer.WriteLine($"        {p.Name}: {val}");
                    }
                    catch
                    {
                        // skip this property if it threw
                    }
                }
            }
            writer.WriteLine();
        }
    }

    private static void DumpStaticDefsLists(StreamWriter writer)
    {
        // find all static “XyzDefs” types that expose a public static List<Def> Defs
        var holders = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
            {
                var prop = t.GetProperty("Defs", BindingFlags.Public | BindingFlags.Static);
                if (prop == null) return false;
                if (!typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)) return false;
                if (!prop.PropertyType.IsGenericType) return false;
                var elem = prop.PropertyType.GetGenericArguments()[0];
                return typeof(Def).IsAssignableFrom(elem);
            })
            .OrderBy(t => t.Name);

        foreach (var holder in holders)
        {
            writer.WriteLine($"Defs holder: {holder.Name}");
            writer.WriteLine();

            var prop = holder.GetProperty("Defs", BindingFlags.Public | BindingFlags.Static);
            var elemType = prop.PropertyType.GetGenericArguments()[0];

            // 1) Try the static Holder.Defs
            IEnumerable list = null;
            try
            {
                list = prop.GetValue(null) as IEnumerable;
            }
            catch
            {
                // swallow and fallback below
            }

            // 2) If it threw, returned null, or returned empty, fall back to DefDatabase<T>.AllDefs
            bool needFallback = false;
            if (list == null) needFallback = true;
            else
            {
                // check if empty
                var en = list.GetEnumerator();
                if (!en.MoveNext()) needFallback = true;
            }

            if (needFallback)
            {
                var dbType = typeof(DefDatabase<>).MakeGenericType(elemType);
                var allDefsProp = dbType.GetProperty("AllDefs", BindingFlags.Public | BindingFlags.Static);
                if (allDefsProp != null)
                {
                    try
                    {
                        list = allDefsProp.GetValue(null) as IEnumerable;
                    }
                    catch
                    {
                        list = null;
                    }
                }
            }

            if (list == null)
            {
                writer.WriteLine("  (no definitions available)\n");
                continue;
            }

            // prepare to reflect on the element properties
            var props = elemType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic)
                .ToList();

            // dump each def
            foreach (var def in list)
            {
                // header
                object defName = "", label = "";
                try
                {
                    defName = elemType.GetProperty("DefName").GetValue(def);
                    label = elemType.GetProperty("LabelCap").GetValue(def);
                }
                catch
                {
                    // ignore missing fields
                }
                writer.WriteLine($"  - {defName} ({label})");

                // individual properties
                foreach (var p in props)
                {
                    try
                    {
                        var val = p.GetValue(def);
                        writer.WriteLine($"      {p.Name}: {val}");
                    }
                    catch
                    {
                        // skip any broken property
                    }
                }

                writer.WriteLine();
            }
        }
    }
}
