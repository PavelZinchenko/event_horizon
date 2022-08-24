using GameServices.Player;

namespace GameModel
{
	namespace Skills
	{
		public class EventListener
		{
			public EventListener(PlayerSkills playerSkills)
			{
                //_playerSkills = playerSkills;
			}
			
			public void Subscribe()
			{
                // TODO:
				//ServiceLocator.Messenger.AddListener<CombatResult>(EventType.CombatCompleted, OnCombatCompleted, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener(EventType.PlanetExplored, OnPlanetExplored, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener(EventType.RuinsExplored, OnRuinsExplored, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener(EventType.ChallengeCompleted, OnChallengeCompleted, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener(EventType.SurvivalCompleted, OnSurvivalCompleted, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener<int>(EventType.BossDefeated, OnBossDefeated, SubscribtionLifeTime.Session);
				//ServiceLocator.Messenger.AddListener<Region>(EventType.BaseCaptured, OnBaseCaptured, SubscribtionLifeTime.Session);
			}

			//private void OnCombatCompleted(CombatResult result)
			//{
			//	if (!result.Victory || result.Rules.CombatType == Model.Military.CombatType.Training || result.Rules.RewardMode == Model.Military.RewardMode.None)
			//		return;

   //             _playerSkills.Experience += 50 + Game.Session.CurrentStar.Level/2;
			//}

			//private void OnPlanetExplored()
			//{
   //             _playerSkills.Experience += 100 + Game.Session.CurrentStar.Level;
			//}

			//private void OnRuinsExplored()
			//{
   //             _playerSkills.Experience += 50 + Game.Session.CurrentStar.Level/2;
			//}

			//private void OnChallengeCompleted()
			//{
   //             _playerSkills.Experience += 50 + Game.Session.CurrentStar.Level/2;
			//}

			//private void OnSurvivalCompleted()
			//{
   //             _playerSkills.Experience += 50 + Game.Session.CurrentStar.Level/2;
			//}

			//private void OnBossDefeated(int level)
			//{
   //             _playerSkills.Experience += 50 + level/2;
			//}

			//private void OnBaseCaptured(Region region)
			//{
   //             _playerSkills.Experience += 100 + region.MilitaryPower;
			//}

		 //   private PlayerSkills _playerSkills;

		}
	}
}
