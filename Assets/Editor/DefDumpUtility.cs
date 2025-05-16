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

/// <summary>
/// Editor utility to dump all loaded Def classes along with their properties, fields, summaries, and instances to a text file.
/// </summary>
public static class DefDumpUtility
{
    // keyed by "ClassName.MemberName" and "ClassName.__class"
    private static Dictionary<string, string> _summaries;

    [MenuItem("Tools/Dump All Defs to Text…")]
    public static void DumpAllDefs()
    {
        // ensure all databases are fresh
        DefDatabaseRegistry.ClearAllDatabases();
        DefDatabaseRegistry.AddAllDefs();
        DefDatabaseRegistry.ResolveAllReferences();
        DefDatabaseRegistry.OnLoadingDone();

        _summaries = BuildSourceSummaries();

        var path = EditorUtility.SaveFilePanel(
            "Save Def Dump", "", "AllDefsDump.txt", "txt"
        );
        if (string.IsNullOrEmpty(path)) return;

        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.WriteLine("=== DEF CLASSES ===\n");
        DumpDefTypes(writer);

        AssetDatabase.Refresh();
        Debug.Log($"Def dump written to: {path}");
    }

    /// <summary>
    /// Parses all .cs files under Assets for XML <summary> comments on classes, properties, and fields.
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

                // look ahead for the member or class declaration
                for (int j = i + 1; j < lines.Length; j++)
                {
                    var line = lines[j].Trim();

                    // match property
                    var pm = Regex.Match(line, @"public\s+[^\s]+\s+(?<prop>\w+)\s*\{");
                    if (pm.Success)
                    {
                        var className = FindEnclosingClass(lines, j);
                        if (className != null)
                        {
                            var key = $"{className}.{pm.Groups["prop"].Value}";
                            if (!dict.ContainsKey(key)) dict[key] = summary;
                        }
                        break;
                    }

                    // match field
                    var fm = Regex.Match(line, @"public\s+[^\s]+\s+(?<field>\w+)\s*(;|=)");
                    if (fm.Success)
                    {
                        var className = FindEnclosingClass(lines, j);
                        if (className != null)
                        {
                            var key = $"{className}.{fm.Groups["field"].Value}";
                            if (!dict.ContainsKey(key)) dict[key] = summary;
                        }
                        break;
                    }

                    // match class
                    var cm = Regex.Match(line, @"class\s+(?<type>\w+)");
                    if (cm.Success)
                    {
                        var key = $"{cm.Groups["type"].Value}.__class";
                        if (!dict.ContainsKey(key)) dict[key] = summary;
                        break;
                    }
                }
            }
        }
        return dict;
    }

    /// <summary>
    /// Searches backwards from line j to find the nearest "class Xyz" declaration.
    /// </summary>
    private static string FindEnclosingClass(string[] lines, int j)
    {
        for (int k = j; k >= 0; k--)
        {
            var cm = Regex.Match(lines[k], @"class\s+(?<type>\w+)");
            if (cm.Success) return cm.Groups["type"].Value;
        }
        return null;
    }

    /// <summary>
    /// Retrieves the XML summary for the given type/member, if any.
    /// </summary>
    private static string GetSummary(Type t, string member)
    {
        var key = $"{t.Name}.{member}";
        return _summaries != null && _summaries.TryGetValue(key, out var s) ? s : "";
    }

    /// <summary>
    /// Formats a raw property/field value for display, handling IEnumerables and Def references.
    /// </summary>
    private static string FormatValue(object rawVal)
    {
        if (rawVal == null) return "";
        if (rawVal is string s) return $"\"{s}\"";
        if (rawVal is Def d) return d.DefName;
        if (rawVal is IEnumerable e && !(rawVal is string))
        {
            var items = new List<string>();
            foreach (var x in e)
            {
                items.Add(FormatValue(x));
            }
            return "[" + string.Join(", ", items) + "]";
        }
        return rawVal.ToString();
    }

    /// <summary>
    /// Reflects over all Def-derived types, dumps their class summary, members and all instances.
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

            // collect public instance properties and fields
            var props = defType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic)
                .Cast<MemberInfo>();

            var fields = defType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Cast<MemberInfo>();

            var members = props.Concat(fields).ToList();

            writer.WriteLine("  Members:");
            foreach (var m in members)
            {
                string name, typeName, summary;

                if (m is PropertyInfo pi)
                {
                    name = pi.Name;
                    typeName = pi.PropertyType.Name;
                }
                else // FieldInfo fi
                {
                    var fi = (FieldInfo)m;
                    name = fi.Name;
                    typeName = fi.FieldType.Name;
                }
                summary = GetSummary(defType, name);
                writer.Write($"    {typeName} {name}");
                if (!string.IsNullOrEmpty(summary))
                    writer.Write($": {summary}");

                // we can't read instance values here (no def instance), so skip "= ..." for class-level member list
                writer.WriteLine();
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

            // iterate instances
            foreach (var def in allDefs)
            {
                object defName = "", label = "";
                try
                {
                    defName = defType.GetProperty("DefName").GetValue(def);
                    label = defType.GetProperty("LabelCap").GetValue(def);
                }
                catch { }

                writer.WriteLine($"    - {defName} ({label})");
                // dump each member's value
                foreach (var m in members)
                {
                    try
                    {
                        object val = m is PropertyInfo pi
                            ? pi.GetValue(def)
                            : ((FieldInfo)m).GetValue(def);
                        var formatted = FormatValue(val);
                        if (!string.IsNullOrEmpty(formatted))
                            writer.WriteLine($"        {m.Name}: {formatted}");
                    }
                    catch
                    {
                        // skip on error
                    }
                }
                writer.WriteLine();
            }
        }
    }
}
