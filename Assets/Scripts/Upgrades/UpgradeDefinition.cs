namespace PlanetCore
{
    public enum UpgradeEffectType
    {
        EPSMultiplier,
        EPCMultiplier,
        StructureStat
    }

    public enum UnlockConditionType
    {
        EPS,
        Capacity,
        StructureCount
    }

    public sealed class UpgradeDefinition
    {
        public string              UpgradeId    { get; }
        public string              DisplayName  { get; }
        public float               Cost         { get; }
        public UnlockConditionType UnlockType   { get; }
        public float               UnlockValue  { get; }
        public UpgradeEffectType   EffectType   { get; }
        public string              EffectTarget { get; }
        public float               EffectValue  { get; }
        public string              SpriteName   { get; }
        public string              Description  { get; }

        public UpgradeDefinition(
            string upgradeId, string displayName, float cost,
            UnlockConditionType unlockType, float unlockValue,
            UpgradeEffectType effectType, string effectTarget, float effectValue,
            string spriteName, string description)
        {
            UpgradeId    = upgradeId;
            DisplayName  = displayName;
            Cost         = cost;
            UnlockType   = unlockType;
            UnlockValue  = unlockValue;
            EffectType   = effectType;
            EffectTarget = effectTarget;
            EffectValue  = effectValue;
            SpriteName   = spriteName;
            Description  = description;
        }

        public string EffectDescription => EffectType switch
        {
            UpgradeEffectType.EPSMultiplier => $"+{EffectValue * 100:F0}% EPS",
            UpgradeEffectType.EPCMultiplier => $"+{EffectValue * 100:F0}% EPC",
            UpgradeEffectType.StructureStat => $"+{EffectValue:F0} 용량",
            _                               => string.Empty
        };
    }
}