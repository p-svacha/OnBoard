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
    // keyed by "ClassName.PropertyName" or "ClassName.__class"
    private static Dictionary<string, string> _summaries;

    [MenuItem("Tools/Dump All Defs to Text…")]
    public static void DumpAllDefs()
    {
        _summaries = BuildSourceSummaries();

        string path = EditorUtility.SaveFilePanel(
            title: "Save Def Dump",
            directory: "",
            defaultName: "AllDefsDump.txt",
            extension: "txt"
        );
        if (string.IsNullOrEmpty(path)) return;

        using var writer = new StreamWriter(path, false, Encoding.UTF8);

        writer.WriteLine("=== DEF CLASSES ===");
        writer.WriteLine();
        DumpDefTypes(writer);

        writer.WriteLine();
        writer.WriteLine("=== STATIC XyzDefs LISTS ===");
        writer.WriteLine();
        DumpStaticDefsLists(writer);

        AssetDatabase.Refresh();
        Debug.Log($"Def dump written to: {path}");
    }

    /// <summary>
    /// Scans all .cs under Assets/ and extracts every /// <summary>…</summary> block,
    /// mapping it either to the next property found or to a class definition.
    /// </summary>
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
                // collect summary text
                var sb = new StringBuilder();
                i++;
                while (i < lines.Length && !lines[i].TrimStart().StartsWith("/// </summary>"))
                {
                    var t = lines[i].Trim();
                    if (t.StartsWith("///"))
                        t = t.Substring(3).Trim();
                    sb.AppendLine(t);
                    i++;
                }
                var summary = sb.ToString().Replace("\r", "").Replace("\n", " ").Trim();

                // look ahead for either a property or a class
                int j = i + 1;
                bool found = false;
                for (; j < lines.Length && !found; j++)
                {
                    var line = lines[j].Trim();
                    // property?
                    var pm = Regex.Match(line, @"public\s+[^\s]+\s+(?<prop>\w+)\s*\{");
                    if (pm.Success)
                    {
                        // backtrack to find the enclosing class name
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
                        found = true;
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
                        found = true;
                        break;
                    }
                }
            }
        }
        return dict;
    }

    /// <summary>
    /// Helper to fetch a summary for a given class or property.
    /// </summary>
    private static string GetSummary(Type t, string member)
    {
        if (_summaries != null && _summaries.TryGetValue($"{t.Name}.{member}", out var s))
            return s;
        return "";
    }

    /// <summary>
    /// Dumps each Def subclass: first prints class summary, then all public properties
    /// with their summaries, then every instance from DefDatabase.AllDefs with actual values.
    /// </summary>
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

            // properties
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

            // instances
            writer.WriteLine("  Instances:");
            var dbType = typeof(DefDatabase<>).MakeGenericType(defType);
            var allDefsProp = dbType.GetProperty("AllDefs", BindingFlags.Public | BindingFlags.Static);
            var allDefs = allDefsProp?.GetValue(null) as IEnumerable;
            if (allDefs == null)
            {
                writer.WriteLine("    (no instances)");
                writer.WriteLine();
                continue;
            }

            // for each instance, print all props
            foreach (var def in allDefs)
            {
                // header: DefName + Label
                var defName = defType.GetProperty("DefName").GetValue(def);
                var label = defType.GetProperty("LabelCap").GetValue(def);
                writer.WriteLine($"    - {defName} ({label})");
                foreach (var p in props)
                {
                    var val = p.GetValue(def);
                    writer.WriteLine($"        {p.Name}: {val}");
                }
            }
            writer.WriteLine();
        }
    }

    /// <summary>
    /// Finds every static class with a public static List<T> Defs property, then
    /// dumps each entry’s full set of public properties (like above).
    /// </summary>
    private static void DumpStaticDefsLists(StreamWriter writer)
    {
        var holders = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
            {
                var prop = t.GetProperty("Defs", BindingFlags.Public | BindingFlags.Static);
                return prop != null
                    && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)
                    && prop.PropertyType.IsGenericType
                    && typeof(Def).IsAssignableFrom(prop.PropertyType.GetGenericArguments()[0]);
            })
            .OrderBy(t => t.Name);

        foreach (var holder in holders)
        {
            writer.WriteLine($"Defs holder: {holder.Name}");
            writer.WriteLine();

            var prop = holder.GetProperty("Defs", BindingFlags.Public | BindingFlags.Static);
            IEnumerable list = null;
            try { list = prop.GetValue(null) as IEnumerable; }
            catch (Exception e)
            {
                writer.WriteLine($"  Error reading Defs: {e.Message}");
                writer.WriteLine();
                continue;
            }

            if (list == null)
            {
                writer.WriteLine("  (empty list)");
                writer.WriteLine();
                continue;
            }

            // element type and its props
            var elemType = prop.PropertyType.GetGenericArguments()[0];
            var props = elemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Where(p => p.GetMethod != null && p.GetMethod.IsPublic).ToList();

            foreach (var def in list)
            {
                var defName = elemType.GetProperty("DefName").GetValue(def);
                var label = elemType.GetProperty("LabelCap").GetValue(def);
                writer.WriteLine($"  - {defName} ({label})");
                foreach (var p in props)
                {
                    var val = p.GetValue(def);
                    writer.WriteLine($"      {p.Name}: {val}");
                }
            }
            writer.WriteLine();
        }
    }
}
