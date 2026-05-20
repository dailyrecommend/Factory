namespace PlanetCore
{
    public class OwnedCardEntry
    {
        public AbilityCard Card   { get; }
        public ICardEffect Effect { get; }
        public int         Level  { get; private set; } = 1;

        public OwnedCardEntry(AbilityCard card)
        {
            Card   = card;
            Effect = card.EffectFactory();
        }

        public void IncrementLevel() => Level++;
    }
}
