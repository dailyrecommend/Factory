using System.Collections.Generic;

namespace PlanetCore
{
    public class MetaProgressionManager
    {
        private readonly List<PersistedCardRecord> _persisted
            = new List<PersistedCardRecord>();

        public IReadOnlyList<PersistedCardRecord> PersistedCards => _persisted;

        public void SnapshotFromRewardSystem(RewardSystem rewards)
        {
            _persisted.Clear();
            foreach (var entry in rewards.OwnedCards)
                _persisted.Add(new PersistedCardRecord(entry.Card.CardId, entry.Level));
        }

        public void ReapplyToNewSession(CardPool pool, RewardSystem rewards, GameSystems systems)
        {
            foreach (var record in _persisted)
            {
                if (!pool.TryGet(record.CardId, out var card)) continue;

                var entry = new OwnedCardEntry(card);
                entry.Effect.Apply(systems);

                for (int lvl = 2; lvl <= record.Level; lvl++)
                {
                    entry.IncrementLevel();
                    entry.Effect.OnUpgrade(entry.Level, systems);
                }

                rewards.InjectOwnedCard(entry);
            }
        }
    }
}
