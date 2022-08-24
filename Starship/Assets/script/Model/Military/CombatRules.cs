using GameDatabase.Enums;

namespace Model
{
	namespace Military
	{
		public enum RewardType
		{
			Default,
			SpecialOnly,
		}

		//public enum CombatType
		//{
		//	Default,
		//	Training,
		//}

		public enum TimeoutBehaviour
		{
			Decay,
			NextEnemy,
            AllEnemiesThenDraw,
		}

		public struct CombatRules
		{
			public int TimeLimit;
			public RewardType RewardType;
			//public CombatType CombatType;
            public RewardCondition LootCondition;
            public RewardCondition ExpCondition;
            public TimeoutBehaviour TimeoutBehaviour;
			public bool CanSelectShips;
		    public bool CanCallEnemies;
			public bool AsteroidsEnabled;
			public bool PlanetEnabled;
			public bool DisableBonusses;
		    public int InitialEnemies;
            public int MaxEnemies;
        }
	}
}
