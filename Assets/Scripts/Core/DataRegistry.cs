using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetCore
{
    // Single source of truth for all CSV-defined data.
    // Loaded once at startup via GameManager.Awake. Immutable after load.
    public sealed class DataRegistry
    {
        private readonly Dictionary<string, TileDefinition>    _tiles     = new();
        private readonly Dictionary<string, StructureDefinition> _structures = new();

        public IGameConfig Config { get; }

        public IEnumerable<TileDefinition>    AllTiles     => _tiles.Values;
        public IEnumerable<StructureDefinition> AllStructures => _structures.Values;

        public DataRegistry(IGameConfig config)
        {
            Config = config;
            LoadTiles();
            LoadStructures();
        }

        // ── Tile ──────────────────────────────────────────────────────────

        public TileDefinition GetTile(string tileTypeId)
        {
            if (_tiles.TryGetValue(tileTypeId, out var def)) return def;
            return Fail<TileDefinition>($"TileDefinition not found: '{tileTypeId}'");
        }

        public bool TryGetTile(string tileTypeId, out TileDefinition def)
            => _tiles.TryGetValue(tileTypeId, out def);

        // ── Building ──────────────────────────────────────────────────────

        public StructureDefinition GetStructure(string structureId)
        {
            if (_structures.TryGetValue(structureId, out var def)) return def;
            return Fail<StructureDefinition>($"StructureDefinition not found: '{structureId}'");
        }

        public bool TryGetStructure(string structureId, out StructureDefinition def)
            => _structures.TryGetValue(structureId, out def);

        // ── Loaders ───────────────────────────────────────────────────────

        private void LoadTiles()
        {
            foreach (var row in CsvParser.LoadFromResources("Data/tiles"))
            {
                var id      = Require(row, "tileTypeId",  "tiles.csv");
                var name    = Require(row, "displayName", "tiles.csv");
                var prefab  = Require(row, "prefabPath",  "tiles.csv");
                var tags    = new HashSet<string>();

                if (row.TryGetValue("tags", out var tagRaw) && !string.IsNullOrEmpty(tagRaw))
                    foreach (var t in tagRaw.Split('|'))
                        tags.Add(t.Trim());

                _tiles[id] = new TileDefinition(id, name, prefab, tags, new Dictionary<string, string>());
            }

            foreach (var row in CsvParser.LoadFromResources("Data/tile_props"))
            {
                var id  = Require(row, "tileTypeId", "tile_props.csv");
                var key = Require(row, "key",        "tile_props.csv");
                row.TryGetValue("value", out var val);

                if (!_tiles.TryGetValue(id, out var def))
                    Fail<object>($"tile_props.csv: unknown tileTypeId '{id}'");

                def.SetProperty(key, val ?? string.Empty);
            }
        }

        private void LoadStructures()
        {
            foreach (var row in CsvParser.LoadFromResources("Data/structures"))
            {
                var id      = Require(row, "structureId",  "structures.csv");
                var name    = Require(row, "displayName",  "structures.csv");
                var costRaw = Require(row, "cost",         "structures.csv");
                row.TryGetValue("requiredTileTag", out var reqTag);
                var prefab  = Require(row, "prefabPath",   "structures.csv");

                if (!float.TryParse(costRaw,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out float cost))
                    Fail<object>($"structures.csv: cannot parse cost '{costRaw}' for '{id}'");

                _structures[id] = new StructureDefinition(id, name, cost, reqTag, prefab);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static string Require(Dictionary<string, string> row, string key, string file)
        {
            if (row.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v)) return v;
            return Fail<string>($"{file}: missing required column '{key}'");
        }

        private static T Fail<T>(string msg)
        {
            Debug.LogError($"[DataRegistry] {msg}");
            throw new Exception($"[DataRegistry] {msg}");
        }
    }
}