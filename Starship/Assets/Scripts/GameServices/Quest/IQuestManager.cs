using System.Collections.Generic;
using Combat.Domain;
using Domain.Quests;
using GameDatabase.DataModel;

namespace GameServices.Quests
{
    public interface IQuestActionProcessor
    {
        void ShowUiDialog(IUserInteraction userInteraction);
        void StartCombat(ICombatModel model);
        void SuppressOccupant(int starId, bool destroy);
        void StartTrading(ILoot merchantItems);
        void Retreat();
        void SetCharacterRelations(int characterId, int value, bool additive);
        void SetFactionRelations(int starId, int value, bool additive);
        void StartQuest(QuestModel quest);
        void OpenShipyard(Faction faction, int level);
        void CaptureStarBase(int starId, bool capture);
        void ChangeFaction(int starId, Faction faction);
    }

    public interface IQuestManager
    {
        bool ActionRequired { get; }
        void InvokeAction(IQuestActionProcessor processor);
        IEnumerable<IQuest> Quests { get; }
        bool IsQuestObjective(int starId);
        void AbandonQuest(IQuest quest);
        void StartQuest(QuestModel questModel);
    }
}
