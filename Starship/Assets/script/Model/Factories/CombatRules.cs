using GameDatabase.DataModel;
using GameDatabase.Enums;
using Model.Military;

namespace Model
{
	namespace Factories
	{
		public static class CombatRules
		{
			public static Military.CombatRules Default(int level = 1)
			{
				return new Model.Military.CombatRules {
					RewardType = Military.RewardType.Default,
					LootCondition = RewardCondition.Default,
                    ExpCondition = RewardCondition.Default,
					TimeLimit = Maths.Distance.CombatTime(level),
					TimeoutBehaviour = Military.TimeoutBehaviour.NextEnemy,
					CanSelectShips = true,
                    CanCallEnemies = true,
					AsteroidsEnabled = true,
					PlanetEnabled = true,
                    InitialEnemies = 1,
                    MaxEnemies = 3,
				};
			}
		
			public static Military.CombatRules Neutral(int level)
			{
				var rules = Default(level);
				return rules;
			}

			public static Military.CombatRules Capital(GameModel.Region region)
			{
				var level = (int)System.Math.Round(region.MilitaryPower * region.BaseDefensePower);
				var rules = Default(level);
                rules.InitialEnemies = 3;
                rules.MaxEnemies = 5;
				rules.PlanetEnabled = false;
				return rules;
			}

			public static Military.CombatRules Faction(Faction faction, int level)
			{
				var rules = Default(level);
				return rules;
			}

			public static Military.CombatRules Training(int level)
			{
				var rules = Default(level);
				rules.LootCondition = RewardCondition.Always;
				rules.RewardType = Model.Military.RewardType.SpecialOnly;
				return rules;
			}			

			public static Military.CombatRules Flagship(int level)
			{
				var rules = Default(level);
				return rules;
			}

            public static Military.CombatRules Xmas(int level)
            {
                var rules = Default(level);
                rules.PlanetEnabled = false;
                rules.InitialEnemies = 5;
                return rules;
            }

            public static Military.CombatRules Arena()
			{
				var rules = Default();
                rules.TimeoutBehaviour = TimeoutBehaviour.AllEnemiesThenDraw;
				rules.RewardType = Model.Military.RewardType.SpecialOnly;
			    rules.TimeLimit = 90;
			    rules.MaxEnemies = 12;
                rules.ExpCondition = RewardCondition.Never;
                rules.DisableBonusses = true;
                rules.CanSelectShips = false;
                rules.AsteroidsEnabled = false;
                return rules;
			}

			public static Military.CombatRules Challenge()
			{
				var rules = Default();
				rules.RewardType = Model.Military.RewardType.SpecialOnly;
				rules.TimeoutBehaviour = Military.TimeoutBehaviour.Decay;
			    rules.ExpCondition = RewardCondition.Never;
				rules.DisableBonusses = true;
				rules.CanSelectShips = false;
				return rules;
			}

			public static Military.CombatRules Multiplayer()
			{
				var rules = Default();
				rules.LootCondition = RewardCondition.Never;
			    rules.ExpCondition = RewardCondition.Never;
                rules.TimeoutBehaviour = Military.TimeoutBehaviour.Decay;
				rules.AsteroidsEnabled = false;
				return rules;
			}

			public static Military.CombatRules Survival(int level)
			{
				var rules = Default(level + 10);
			    rules.LootCondition = RewardCondition.Always;
			    rules.ExpCondition = RewardCondition.Always;
                return rules;
			}
		}
	}
}
