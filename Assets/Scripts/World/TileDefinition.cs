using System.Collections.Generic;
using System.Globalization;

namespace PlanetCore
{
    public sealed class TileDefinition
    {
        public string          TileTypeId  { get; }
        public string          DisplayName { get; }
        public string          PrefabPath  { get; }
        public HashSet<string> Tags        { get; }

        private readonly Dictionary<string, string> _properties;
        public IReadOnlyDictionary<string, string>  Properties => _properties;

        public TileDefinition(
            string tileTypeId,
            string displayName,
            string prefabPath,
            HashSet<string> tags,
            Dictionary<string, string> properties)
        {
            TileTypeId  = tileTypeId;
            DisplayName = displayName;
            PrefabPath  = prefabPath;
            Tags        = tags;
            _properties = properties;
        }

        public bool HasTag(string tag) => Tags.Contains(tag);

        public bool TryGetProperty(string key, out string value)
            => _properties.TryGetValue(key, out value);

        public float GetFloat(string key, float fallback = 0f)
        {
            if (!_properties.TryGetValue(key, out var raw)) return fallback;
            return float.TryParse(raw, NumberStyles.Float,
                CultureInfo.InvariantCulture, out float v) ? v : fallback;
        }

        internal void SetProperty(string key, string value)
            => _properties[key] = value;
    }
}