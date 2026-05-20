using System.Collections.Generic;

namespace PlanetCore
{
    public class CardPool
    {
        private readonly Dictionary<string, AbilityCard> _pool
            = new Dictionary<string, AbilityCard>();

        public CardPool()
        {
            Register(new AbilityCard(
                "chunk_expansion", "Chunk Expansion", CardGrade.Common,
                "Unlock one adjacent 5x5 chunk.",
                0f, () => new ChunkExpansionEffect()));

            Register(new AbilityCard(
                "time_accelerator", "Time Accelerator", CardGrade.Uncommon,
                "Enable 2x/3x speed controls.",
                0f, () => new TimeAcceleratorEffect()));

            Register(new AbilityCard(
                "pioneering_luck", "Pioneering Luck", CardGrade.Rare,
                "New chunks are guaranteed at least 1 coal deposit.",
                200f, () => new PioneeringLuckEffect()));
        }

        public void Register(AbilityCard card) => _pool[card.CardId] = card;

        public bool TryGet(string id, out AbilityCard card)
            => _pool.TryGetValue(id, out card);

        public IReadOnlyCollection<AbilityCard> AllCards
            => (IReadOnlyCollection<AbilityCard>)_pool.Values;
    }
}
