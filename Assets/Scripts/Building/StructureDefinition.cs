namespace PlanetCore
{
    // Static definition loaded from structures.csv.
    public sealed class StructureDefinition
    {
        public string StructureId     { get; }
        public string DisplayName     { get; }
        public float  Cost            { get; }
        public string RequiredTileTag { get; }
        public string PrefabPath      { get; }

        public bool HasTileRequirement => !string.IsNullOrEmpty(RequiredTileTag);

        public StructureDefinition(
            string structureId,
            string displayName,
            float  cost,
            string requiredTileTag,
            string prefabPath)
        {
            StructureId     = structureId;
            DisplayName     = displayName;
            Cost            = cost;
            RequiredTileTag = requiredTileTag ?? string.Empty;
            PrefabPath      = prefabPath;
        }
    }
}