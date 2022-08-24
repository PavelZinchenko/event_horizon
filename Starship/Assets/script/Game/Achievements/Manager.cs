using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Economy.Products;

namespace GameModel.Achievements
{
	public class Manager
	{
	    public void Initialize()
	    {
            //ServiceLocator.Messenger.AddListener<IAchievement>(EventType.AchievementUnlocked, OnAchievementUnlocked);

            foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
	        {
	            // TODO: var comleted = Game.Session.GameData.Achievements.IsUnlocked(type);
	            // var achievement = type.Create(comleted);

	            //if (achievement.Completed)
	            //    _completed.Add(achievement);
	            //else
	            //    _active.Add(achievement);
	        }
	    }

	    public IAchievement GetRecentAchievement()
	    {
	        return _recent.Count > 0 ? _recent.Dequeue() : null;
	    }

	    private void OnAchievementUnlocked(IAchievement achievement)
	    {
            // TODO:
            //Game.Session.GameData.Achievements.UnlockAchievement(achievement.Type);
            //_active.Remove(achievement);
            //_completed.Add(achievement);
            //_recent.Enqueue(achievement);

            // TODO:
	        //foreach (var item in achievement.Type.GetReward())
	        //    item.Consume();
	    }

        //public const string GiftedStudent	= "CgkIko6QrcUOEAIQAg";
        //public const string Raider			= "CgkIko6QrcUOEAIQAw";
        //public const string NoScratch		= "CgkIko6QrcUOEAIQBA";
        //public const string YourOwnFleet	= "CgkIko6QrcUOEAIQBQ";
        //public const string SecretWeapon	= "CgkIko6QrcUOEAIQBg";


        //public static IEnumerable<string> BossAchievementsDelegate(Combat.CombatData data)
        //{
        //	if (!Game.Session.GameData.Achievements.IsExists(Achievements.Raider) && data.SecondFleet.IsEmpty && !data.FirstFleet.IsEmpty)
        //	{
        //		yield return Achievements.Raider;
        //	}
        //}

        //public static IEnumerable<string> TutorialAchievementsDelegate(Combat.CombatData data)
        //{
        //	if (!Game.Session.GameData.Achievements.IsExists(Achievements.GiftedStudent) && 
        //	    data.SecondFleet.IsEmpty && !data.FirstFleet.Ships.Where(ship => data.FirstFleet.GetCondition(ship) <= 0).Any())
        //	{
        //		yield return Achievements.GiftedStudent;
        //	}
        //}

        //public static IEnumerable<string> CommonAchievementsDelegate(Combat.CombatData data)
        //{
        //	if (!Game.Session.GameData.Achievements.IsExists(Achievements.NoScratch) && 
        //	    data.SecondFleet.IsEmpty && !data.FirstFleet.Ships.Where(ship => data.FirstFleet.GetCondition(ship) < 1).Any())
        //	{
        //		yield return Achievements.NoScratch;
        //	}
        //}

        private readonly HashSet<IAchievement> _completed = new HashSet<IAchievement>();
        private readonly HashSet<IAchievement> _active = new HashSet<IAchievement>();
        private readonly Queue<IAchievement> _recent = new Queue<IAchievement>();
    }
}
