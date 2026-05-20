namespace PlanetCore
{
    public class PersistedCardRecord
    {
        public string CardId { get; }
        public int    Level  { get; }

        public PersistedCardRecord(string cardId, int level)
        {
            CardId = cardId;
            Level  = level;
        }
    }
}
