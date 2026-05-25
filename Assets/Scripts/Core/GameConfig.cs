using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PlanetCore
{
    // Loads constants.csv and exposes values via IGameConfig.
    // CSV format:  key,value
    // Throws immediately on any missing or unparseable required key.
    public sealed class GameConfig : IGameConfig
    {
        public int   ChunkSize      { get; }
        
        public float StartingCredits { get; }
        
        public float DepositChance  { get; }
        

        public float ManualBurstDuration     { get; }
        public float ManualBurstEnergyPerSec { get; }

        public float BaseQuota          { get; }
        public float QuotaGrowthRate    { get; }
        public float BaseRate           { get; }
        public float BonusRate          { get; }

        public float DemolishRefundRatio  { get; }
        public float BuildingCostBase     { get; }
        public float BuildingCostExponent { get; }

        public float DayLengthSeconds { get; }
        public float TileSize         { get; }

        public GameConfig(string resourcePath = "Data/constants")
        {
            var rows = CsvParser.LoadFromResources(resourcePath);
            var map  = new Dictionary<string, string>();

            foreach (var row in rows)
                if (row.TryGetValue("key", out var k) && row.TryGetValue("value", out var v))
                    map[k] = v;

            ChunkSize      = Int  (map, "ChunkSize");
            StartingCredits = Float(map, "StartingCredits");
            DepositChance  = Float(map, "DepositChance");

            ManualBurstDuration     = Float(map, "ManualBurstDuration");
            ManualBurstEnergyPerSec = Float(map, "ManualBurstEnergyPerSec");

            BaseQuota          = Float(map, "BaseQuota");
            QuotaGrowthRate    = Float(map, "QuotaGrowthRate");
            BaseRate           = Float(map, "BaseRate");
            BonusRate          = Float(map, "BonusRate");

            DemolishRefundRatio  = Float(map, "DemolishRefundRatio");
            BuildingCostBase     = Float(map, "BuildingCostBase");
            BuildingCostExponent = Float(map, "BuildingCostExponent");


            DayLengthSeconds = Float(map, "DayLengthSeconds");
            TileSize         = Float(map, "TileSize");
        }

        private static float Float(Dictionary<string, string> map, string key)
        {
            if (!map.TryGetValue(key, out var raw)) Fail(key);
            if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                Fail(key, raw);
            return v;
        }

        private static int Int(Dictionary<string, string> map, string key)
        {
            if (!map.TryGetValue(key, out var raw)) Fail(key);
            if (!int.TryParse(raw, out int v)) Fail(key, raw);
            return v;
        }

        private static void Fail(string key, string raw = null)
        {
            var msg = raw == null
                ? $"[GameConfig] Required key '{key}' missing in constants.csv"
                : $"[GameConfig] Cannot parse '{key}' = '{raw}' in constants.csv";
            Debug.LogError(msg);
            throw new Exception(msg);
        }
    }
}