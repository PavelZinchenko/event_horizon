using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataModel.Technology;
using GameModel.GameData;

namespace GameModel
{
	public static class Craft
	{
		public static int GetWorkshopLevel(ITechnology technology)
		{
		    return technology.Price + technology.Requirements.Sum(item => GetWorkshopLevel(item));
		}

        //public static CraftInfo GetCurrentWork(int workshopId)
        //{
        //	CraftData.WorkInfo info;
        //          if (Game.Session.GameData.Craft.TryGetInfo(workshopId, out info))
        //		return new CraftInfo(info);
        //	return CraftInfo.Empty;
        //}

        //private static void EnsureStartTimeCorrect(int workshopId)
        //{
        //	CraftData.WorkInfo info;
        //          if (!Game.Session.GameData.Craft.TryGetInfo(workshopId, out info))
        //		return;

        //	long buildTime = 0;
        //	foreach (var item in info.BuildQueue)
        //		buildTime += Model.DataBase.Technologies.Get(item).CraftTime;

        //	var current = System.DateTime.UtcNow.Ticks;
        //	if (info.StartTime + buildTime >= current)
        //		return;

        //          Game.Session.GameData.Craft.SetWorkshopInfo(workshopId, info.BuildQueue, current - buildTime);
        //}

        //public static bool ReceiveItems(int workshopId)
        //{
        //	var info = GetCurrentWork(workshopId);

        //	if (!info.RemoveCompletedItems())
        //		return false;

        //          Game.Session.GameData.Craft.SetWorkshopInfo(workshopId, info.Technologies.Select(item => item.Id), info.StartTime);

        //          ServiceLocator.Messenger.Broadcast<int>(EventType.CraftFinished, workshopId);
        //	return true;
        //}

        //public static IEnumerable<int> GetAllActiveWorkshops()
        //{
        //          return Game.Session.GameData.Craft.GetActiveWorkshops();
        //}

        //public struct CraftInfo
        //{
        //          public CraftInfo(CraftData.WorkInfo info)
        //	{
        //		Technologies = new Queue<ITechnology>(info.BuildQueue.Select(item => Model.DataBase.Technologies.Get(item)));
        //		_startTime = info.StartTime;
        //	}

        //	public CraftInfo(IEnumerable<ITechnology> technologies, long startTime)
        //	{
        //		Technologies = new Queue<ITechnology>(technologies);
        //		_startTime = startTime;
        //	}

        //	public static CraftInfo Empty { get { return new CraftInfo(Enumerable.Empty<ITechnology>(), 0); } }

        //	public bool IsEmpty { get { return Technologies.Count == 0; } }

        //	public int Count { get { return Technologies.Count; } }

        //	public bool RemoveCompletedItems()
        //	{
        //		var currentTime = System.DateTime.UtcNow.Ticks;
        //		int count = 0;

        //		while (Technologies.Count > 0)
        //		{
        //			var item = Technologies.Peek();
        //			var craftTime = item.CraftTime;
        //			if (currentTime - StartTime < craftTime)
        //				break;

        //			Technologies.Dequeue();
        //			_startTime += craftTime;
        //			item.CreateItem().Consume();
        //			count++;
        //		}

        //		return count > 0;
        //	}

        //	public IEnumerable<ProgressInfo> ProgressData
        //	{
        //		get
        //		{
        //			var currentTime = System.DateTime.UtcNow.Ticks;
        //			foreach (var tech in Technologies)
        //			{
        //				var craftTime = tech.CraftTime;
        //				var progress = 1000L*(currentTime - StartTime)/craftTime;
        //				var timeLeft = currentTime > StartTime ? craftTime - (currentTime - StartTime) : craftTime;
        //				currentTime -= craftTime;
        //				yield return new ProgressInfo(tech, progress/1000f, timeLeft);
        //			}
        //		}
        //	}

        //	public long StartTime { get { return _startTime; } }

        //	public readonly Queue<ITechnology> Technologies;
        //	private long _startTime;
        //}

        //public struct ProgressInfo
        //{
        //	public ProgressInfo(ITechnology technology, float progress, long timeLeft)
        //	{
        //		Technology = technology;
        //		Progress = Mathf.Clamp01(progress);
        //		TimeLeft = timeLeft > 0 ? timeLeft : 0;
        //	}

        //	public readonly ITechnology Technology;
        //	public readonly float Progress;
        //	public readonly long TimeLeft;
        //}
    }
}
