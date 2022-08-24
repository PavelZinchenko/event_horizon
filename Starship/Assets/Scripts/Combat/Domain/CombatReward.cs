using System.Collections.Generic;
using System.Linq;
using Combat.Component.Unit.Classification;
using Economy.ItemType;
using Economy.Products;
using GameModel;
using GameModel.Quests;
using GameServices.Economy;
using GameServices.Player;
using Model.Military;

namespace Combat.Domain
{
    public class CombatReward : IReward
    {
        public CombatReward(CombatModel combatModel, PlayerSkills playerSkills, LootGenerator lootGenerator, Galaxy.Star currentStar)
        {
            if (combatModel.IsLootAllowed())
            {
                foreach (var item in CreateItems(combatModel, lootGenerator, currentStar))
                {
                    IProduct product;
                    if (_items.TryGetValue(item.Type.Id, out product))
                        _items[item.Type.Id] = new Product(item.Type, item.Quantity + product.Quantity);
                    else
                        _items.Add(item.Type.Id, item);
                }
            }

            PlayerExperience = ExperienceData.Empty;
            if (combatModel.IsExpAllowed())
            {
                var expMultiplier = playerSkills.ExperienceMultiplier;
                foreach (var item in combatModel.PlayerExperience)
                {
                    var exp = (long) (item.Value*expMultiplier);
                    if (exp <= 0)
                        continue;

                    _experience.Add(new ExperienceData(item.Key, exp));
                }

                var totalExp = Experience.Sum(item => item.ExperienceAfter - item.ExperienceBefore);
                PlayerExperience = new ExperienceData(playerSkills.Experience,
                    GameModel.Skills.Experience.ConvertCombatExperience(totalExp, playerSkills.Experience.Level));
            }
        }

        public IEnumerable<IProduct> Items { get { return _items.Values; } }
        public IEnumerable<ExperienceData> Experience { get { return _experience; } }
        public ExperienceData PlayerExperience { get; private set; }

        private IEnumerable<IProduct> CreateItems(CombatModel combatModel, LootGenerator lootGenerator, Galaxy.Star currentStar)
        {
            if (combatModel.SpecialRewards != null)
                foreach (var item in combatModel.SpecialRewards)
                    yield return item;

            if (combatModel.Rules.RewardType == RewardType.SpecialOnly)
                yield break;

            var rewards = lootGenerator.GetCommonReward(combatModel.EnemyFleet.Ships.Where(item => item.Status == ShipStatus.Destroyed).Select(item => item.ShipData),
                currentStar.Level, currentStar.Region.Faction, currentStar.Id);
            foreach (var item in rewards)
                yield return item;
        }

        private readonly Dictionary<string, IProduct> _items = new Dictionary<string, IProduct>();
        private readonly List<ExperienceData> _experience = new List<ExperienceData>();
    }
}
