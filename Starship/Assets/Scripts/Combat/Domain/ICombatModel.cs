using Combat.Component.Unit.Classification;
using GameDatabase.Enums;
using GameModel.Quests;
using GameServices.Economy;
using GameServices.Player;
using Model.Military;

namespace Combat.Domain
{
    public interface ICombatModel
    {
        CombatRules Rules { get; }

        IReward GetReward(LootGenerator lootGenerator, PlayerSkills playerSkills, Galaxy.Star currentStar);

        IFleetModel PlayerFleet { get; }
        IFleetModel EnemyFleet { get; }
    }

    public static class CombatModelExtensions
    {
        public static bool IsCompleted(this ICombatModel combatModel)
        {
            return !combatModel.EnemyFleet.IsAnyShipAlive() || !combatModel.PlayerFleet.IsAnyShipAlive();
        }

        public static UnitSide GetWinner(this ICombatModel combatModel)
        {
            if (!combatModel.PlayerFleet.IsAnyShipAlive())
                return UnitSide.Enemy;
            if (combatModel.EnemyFleet.IsAnyShipAlive())
                return UnitSide.Enemy;

            return UnitSide.Player;
        }

        public static bool IsLootAllowed(this ICombatModel combatModel)
        {
            switch (combatModel.Rules.LootCondition)
            {
                case RewardCondition.Always:
                    return true;
                case RewardCondition.Default:
                    return combatModel.GetWinner() == UnitSide.Player;
                case RewardCondition.Never:
                default:
                    return false;
            }
        }

        public static bool IsExpAllowed(this ICombatModel combatModel)
        {
            switch (combatModel.Rules.ExpCondition)
            {
                case RewardCondition.Always:
                    return true;
                case RewardCondition.Default:
                    return combatModel.GetWinner() == UnitSide.Player;
                case RewardCondition.Never:
                default:
                    return false;
            }
        }
    }
}
