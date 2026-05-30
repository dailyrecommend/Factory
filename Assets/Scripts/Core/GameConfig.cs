using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PlanetCore
{
    public sealed class GameConfig : IGameConfig
    {
        public int   ChunkSize               { get; }
        public int   StartingCredit { get; }
        public float DepositChance           { get; }
        public float ManualBurstDuration     { get; }
        public float ManualBurstEnergyPerSec { get; }
        public float DemolishRefundRatio     { get; }
        public float BuildingCostExponent    { get; }
        public float TileSize                { get; }
        public float SellInterval            { get; }
        public float EnergyToCredits         { get; }

        public GameConfig(string resourcePath = "Data/constants")
        {
            var rows = CsvParser.LoadFromResources(resourcePath);
            var map  = new Dictionary<string, string>();

            foreach (var row in rows)
                if (row.TryGetValue("key", out var k) && row.TryGetValue("value", out var v))
                    map[k] = v;

            ChunkSize               = Int  (map, "ChunkSize");
            StartingCredit          = Int  (map, "StartingCredit");
            DepositChance           = Float(map, "DepositChance");
            ManualBurstDuration     = Float(map, "ManualBurstDuration");
            ManualBurstEnergyPerSec = Float(map, "ManualBurstEnergyPerSec");
            DemolishRefundRatio     = Float(map, "DemolishRefundRatio");
            BuildingCostExponent    = Float(map, "BuildingCostExponent");
            TileSize                = Float(map, "TileSize");
            SellInterval            = Float(map, "SellInterval");
            EnergyToCredits         = Float(map, "EnergyToCredits");
        }

        private static float Float(Dictionary<string, string> map, string key)
        {
            if (!map.TryGetValue(key, out var raw)) Fail(key);
            if (!float.TryParse(raw, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out float v)) Fail(key, raw);
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