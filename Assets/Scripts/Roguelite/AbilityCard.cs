using System;

namespace PlanetCore
{
    public class AbilityCard
    {
        public string    CardId              { get; }
        public string    DisplayName         { get; }
        public CardGrade Grade               { get; }
        public string    Description         { get; }
        public float     UpgradeCostPerLevel { get; }
        public Func<ICardEffect> EffectFactory { get; }

        public AbilityCard(string cardId, string displayName, CardGrade grade,
            string description, float upgradeCost, Func<ICardEffect> factory)
        {
            CardId              = cardId;
            DisplayName         = displayName;
            Grade               = grade;
            Description         = description;
            UpgradeCostPerLevel = upgradeCost;
            EffectFactory       = factory;
        }
    }
}
