using System;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using Session.Content;

namespace Domain.Quests
{
    public enum QuestStatus
    {
        InProgress,
        ActionRequired,
        Completed,
        Failed,
        Error,
        Cancelled,
    }

    public static class QuestExtensions
    {
        public static bool IsFinished(this QuestStatus status)
        {
            return status == QuestStatus.Completed || status == QuestStatus.Error || status == QuestStatus.Failed || status == QuestStatus.Cancelled;
        }

        public static bool IsCancellable(this QuestType type)
        {
            return type == QuestType.Common || type == QuestType.Singleton || type == QuestType.Temporary;
        }

        public static bool IsOnCooldown(this QuestModel model, QuestData questData, long currentTime, System.Random random)
        {
            if (model.StartCondition == StartCondition.Daily)
            {
                var lastStartTime = questData.LastStartTime(model.Id.Value);
                int minCooldown;

                if (model.Weight < 0.001)
                {
                    minCooldown = 20*60; // 20 hours
                }
                else if (model.Weight > 1440/5)
                {
                    minCooldown = 5;
                }
                else
                {
                    minCooldown = (int)(1440 / model.Weight);
                }

                minCooldown += random.Next(minCooldown/2 + 1) - minCooldown/4;

                return currentTime - lastStartTime < TimeSpan.TicksPerMinute * minCooldown;
            }

            return false;
        }

        public static bool CanBeStarted(this QuestModel model, QuestData questData, int starId)
        {
            switch (model.QuestType)
            {
                case QuestType.Temporary:
                    return true;
                case QuestType.Common:
                case QuestType.Urgent:
                    return !questData.IsQuestActive(model.Id.Value, starId);
                case QuestType.Singleton:
                    return !questData.IsQuestActive(model.Id.Value);
                case QuestType.Storyline:
                    return !questData.IsActiveOrCompleted(model.Id.Value);
                default:
                    throw new ArgumentException();
            }
        }

        public static bool IsFactionMission(this IQuest quest, int starId)
        {
            return quest.Model.StartCondition == StartCondition.FactionMission && quest.StarId == starId;
        }
    }

    public interface IQuest
    {
        QuestModel Model { get; }
        int StarId { get; }
        string GetRequirementsText(ILocalization localization);
        bool TryGetBeacons(ICollection<int> beacons);

        QuestStatus Status { get; }
    }
}
