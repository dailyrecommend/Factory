using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanetCore
{
    public class RewardSystem
    {
        private readonly CardPool     _pool;
        private readonly EconomyState _economy;
        private GameSystems           _systems;

        private readonly List<OwnedCardEntry> _owned  = new List<OwnedCardEntry>();
        private List<AbilityCard>             _offer  = new List<AbilityCard>();
        private int                           _rerolls = 0;

        public IReadOnlyList<OwnedCardEntry> OwnedCards   => _owned;
        public IReadOnlyList<AbilityCard>    CurrentOffer => _offer;

        public RewardSystem(CardPool pool, EconomyState economy)
        {
            _pool    = pool;
            _economy = economy;
        }

        public void SetSystems(GameSystems systems) => _systems = systems;

        public void GenerateOffer(Random rng)
        {
            var ownedIds = new HashSet<string>(_owned.Select(e => e.Card.CardId));
            var eligible = _pool.AllCards.Where(c => !ownedIds.Contains(c.CardId)).ToList();

            Shuffle(eligible, rng);
            _offer = eligible.Take(3).ToList();
        }

        public bool TryPickCard(string cardId, TurnManager turnManager)
        {
            var picked = _offer.FirstOrDefault(c => c.CardId == cardId);
            if (picked == null) return false;

            var entry = new OwnedCardEntry(picked);
            entry.Effect.Apply(_systems);
            _owned.Add(entry);

            _offer.Clear();
            _rerolls = 0;

            turnManager.ResumeFromReward();
            return true;
        }

        public float NextRerollCost()
            => GameConstants.RerollBaseCost
               * (float)Math.Pow(GameConstants.RerollCostMultiplier, _rerolls);

        public bool TryReroll(Random rng)
        {
            float cost = NextRerollCost();
            if (_economy.Credits < cost) return false;

            _economy.SpendCredits(cost);
            _rerolls++;
            GenerateOffer(rng);
            return true;
        }

        public bool TryUpgradeCard(string cardId)
        {
            var entry = _owned.FirstOrDefault(e => e.Card.CardId == cardId);
            if (entry == null) return false;

            float cost = entry.Card.UpgradeCostPerLevel;
            if (_economy.Credits < cost) return false;

            _economy.SpendCredits(cost);
            entry.IncrementLevel();
            entry.Effect.OnUpgrade(entry.Level, _systems);
            return true;
        }

        public void InjectOwnedCard(OwnedCardEntry entry) => _owned.Add(entry);

        public void SessionReset()
        {
            _rerolls = 0;
            _offer.Clear();
            _owned.Clear();
        }

        private static void Shuffle<T>(List<T> list, Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
