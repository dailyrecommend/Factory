using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    // Generic CSV parser. First row = header. '#' lines and blank lines skipped.
    // Supports double-quoted fields (commas and newlines inside quotes are safe).
    public static class CsvParser
    {
        public static List<Dictionary<string, string>> Parse(string csvText)
        {
            var results = new List<Dictionary<string, string>>();
            if (string.IsNullOrWhiteSpace(csvText)) return results;

            var lines   = csvText.Split('\n');
            string[] headers = null;

            foreach (var raw in lines)
            {
                var line = raw.TrimEnd('\r').Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                var cols = SplitLine(line);

                if (headers == null) { headers = cols; continue; }

                var row = new Dictionary<string, string>();
                for (int i = 0; i < headers.Length; i++)
                    row[headers[i]] = i < cols.Length ? cols[i].Trim() : string.Empty;

                results.Add(row);
            }

            return results;
        }

        // Loads a TextAsset from Resources and parses it. Throws if not found.
        public static List<Dictionary<string, string>> LoadFromResources(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            if (asset != null) return Parse(asset.text);

            var msg = $"[CsvParser] Missing CSV at Resources/{resourcePath}";
            Debug.LogError(msg);
            throw new Exception(msg);
        }

        private static string[] SplitLine(string line)
        {
            var fields  = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    { current.Append('"'); i++; }
                    else
                    { inQuotes = !inQuotes; }
                }
                else if (c == ',' && !inQuotes)
                { fields.Add(current.ToString()); current.Clear(); }
                else
                { current.Append(c); }
            }

            fields.Add(current.ToString());
            return fields.ToArray();
        }
    }
}