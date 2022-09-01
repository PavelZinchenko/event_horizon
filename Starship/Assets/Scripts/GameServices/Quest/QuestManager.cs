using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Quests;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameModel;
using Services.InternetTime;
using Session;
using Utils;
using Zenject;

namespace GameServices.Quests
{
	public class QuestManager : GameServiceBase, IQuestManager, ITickable
	{
		[Inject] private readonly QuestFactory _factory;
	    [Inject] private readonly RequirementsFactory _requirementsFactory;
        [Inject] private readonly GameTime _gameTime;

        [Inject]
	    public QuestManager(
			ISessionData session,
            IDatabase database,
			QuestActionRequiredSignal.Trigger questActionRequiredTrigger,
            QuestListChangedSignal.Trigger questListChangedTrigger,
            QuestEventSignal questEventSignal,
            StarContentChangedSignal.Trigger starContentChangedTrigger,
            SessionDataLoadedSignal dataLoadedSignal,
            SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
	    {
	        _session = session;
	        _database = database;
            _questListChangedTrigger = questListChangedTrigger;
			_questActionRequiredTrigger = questActionRequiredTrigger;
	        _starContentChangedTrigger = starContentChangedTrigger;
	        _questEventSignal = questEventSignal;
	        _questEventSignal.Event += OnQuestEvent;
	    }

	    public bool ActionRequired { get; private set; }

	    public void InvokeAction(IQuestActionProcessor processor)
	    {
	        if (_activeQuest != null && _activeQuest.TryInvokeAction(processor))
                _recentlyUpdatedQuests.Add(_activeQuest);
	    }

	    public IEnumerable<IQuest> Quests { get { return _quests.Cast<IQuest>(); } }

        public bool IsQuestObjective(int starId)
        {
            return _questBeacons.Count > 0 && _questBeacons.Contains(starId);
        }

	    public void Tick()
	    {
	        if (!_session.IsGameStarted) return;

            var counter = 100;
            while (_recentlyUpdatedQuests.Count > 0)
            {
                if (--counter == 0)
                {
                    var currentQuest = _recentlyUpdatedQuests.Last();
                    UnityEngine.Debug.LogException(new InvalidOperationException(
                        "QuestManager - infinite loop. Quest:" + currentQuest.Id + " Node:" + currentQuest.NodeId));
                    UnityEngine.Debug.Break();
                    break;
                }

                var index = _recentlyUpdatedQuests.Count - 1;
                var quest = _recentlyUpdatedQuests[index];
                _recentlyUpdatedQuests.RemoveAt(index);
                OnQuestUpdated(quest);
            }

            if (_gameTime.TotalPlayTime - _lastUpdateTime > UpdateCooldown)
            {
                _lastUpdateTime = _gameTime.TotalPlayTime;
				OnQuestEvent(new SimpleEventData(QuestEventType.Timer));
            }
        }

        public void AbandonQuest(IQuest quest)
	    {
	        var result = _quests.Find(item => item == quest);
	        if (result == null) return;

    	    CompleteQuest(result);
	        UpdateQuestBeacons();
        }

        protected override void OnSessionDataLoaded()
	    {
	        _quests.Clear();
	        _questBeacons.Clear();
            _questBeaconsOld.Clear();
            _recentlyUpdatedQuests.Clear();
	        _activeQuest = null;
            _lastUpdateTime = 0;

            _beaconQuests.Clear();
            _localEncounterQuests.Clear();
            _arrivedAtStarQuests.Clear();
            _newStarExploredQuests.Clear();
            _factionQuests.Clear();
            _dailyQuests.Clear();
        }

        protected override void OnSessionCreated()
	    {
	        foreach (var item in _session.Quests.GetActiveQuests())
	        {
                Add(_factory.Create(item));
	        }

	        var starId = _session.StarMap.PlayerPosition;
	        var seed = _session.Game.Seed;

            foreach (var questData in _database.QuestList)
                if (questData.StartCondition == StartCondition.GameStart && questData.CanBeStarted(_session.Quests, starId))
	                Add(_factory.Create(questData, starId, seed + questData.Id.Value));

            _beaconQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.Beacon), _requirementsFactory, seed);
	        _localEncounterQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.LocalEncounter), _requirementsFactory, seed);
	        _arrivedAtStarQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.ArrivedAtStar), _requirementsFactory, seed);
	        _newStarExploredQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.NewStarExplored), _requirementsFactory, seed);
            _factionQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.FactionMission), _requirementsFactory, seed);
			_dailyQuests.Assign(_database.QuestList.Where(item => item.StartCondition == StartCondition.Daily), _requirementsFactory, seed);
        }

	    public void StartQuest(QuestModel questModel)
	    {
	        var starId = _session.StarMap.PlayerPosition;

            // TODO, make option to mod this.
            var seed = new System.Random().Next();

	        if (questModel.StartCondition != StartCondition.Manual)
	        {
	            UnityEngine.Debug.LogException(new ArgumentException("QuestManager.StartQuest: Wrong start condition - " + questModel.StartCondition));
	            return;
	        }

            if (!questModel.CanBeStarted(_session.Quests, starId))
	        {
	            UnityEngine.Debug.LogError(new ArgumentException("QuestManager.StartQuest: Quest can't be started - " + questModel.Id.Value));
                return;
	        }

	        if (!_requirementsFactory.CreateForQuest(questModel.Requirement, seed).CanStart(starId, seed))
	        {
	            UnityEngine.Debug.LogError(new ArgumentException("QuestManager.StartQuest: Requirements are not met - " + questModel.Id.Value));
	            return;
	        }

            Add(_factory.Create(questModel, starId, seed));
	    }

        private void Add(Quest quest)
	    {
	        if (quest == null)
	        {
	            //UnityEngine.Debug.LogException(new ArgumentException("QuestManager: quest is null"));
                return;
	        }

	        UnityEngine.Debug.Log("new quest: " + quest.Model.Name);

	        _quests.Add(quest);
            _recentlyUpdatedQuests.Add(quest);
            _questListChangedTrigger.Fire();
	    }

        private void OnQuestUpdated(Quest quest)
        {
            if (quest.Model.QuestType != QuestType.Temporary)
                _session.Quests.SetQuestProgress(quest.Id, quest.StarId, quest.Seed, quest.NodeId, _gameTime.TotalPlayTime);

            if (quest.Status.IsFinished())
            {
                CompleteQuest(quest);
                FindActiveQuest();
            }
            else if (quest.Status == QuestStatus.ActionRequired)
		    {
		        if (_activeQuest == null || _activeQuest == quest || _activeQuest.Status != QuestStatus.ActionRequired || _quests.IndexOf(quest) > _quests.IndexOf(_activeQuest))
		        {
		            _activeQuest = quest;
		            ActionRequired = true;
		            _questActionRequiredTrigger.Fire();
		        }
            }
            else if (quest == _activeQuest || _activeQuest == null)
            {
                FindActiveQuest();
            }

            UpdateQuestBeacons();
        }

	    private void UpdateQuestBeacons()
	    {
	        var temp = _questBeaconsOld;
	        _questBeaconsOld = _questBeacons;
	        _questBeacons = temp;

	        _questBeacons.Clear();
	        foreach (var quest in _quests)
	            quest.TryGetBeacons(_questBeacons);

            _questBeaconsOld.SymmetricExceptWith(_questBeacons);
	        if (_questBeaconsOld.Count <= 0) return;

	        foreach (var id in _questBeaconsOld)
	            _starContentChangedTrigger.Fire(id);
	    }

        private void FindActiveQuest()
	    {
	        _activeQuest = null;
	        ActionRequired = false;

	        for (var i = _quests.Count - 1; i >= 0; --i)
	        {
	            var item = _quests[i];
	            if (item.Status != QuestStatus.ActionRequired)
                    continue;

                _activeQuest = item;
	            ActionRequired = true;
	            _questActionRequiredTrigger.Fire();
	            break;
	        }
	    }

        private void CompleteQuest(Quest quest)
	    {
	        _quests.Remove(quest);
	        _recentlyUpdatedQuests.Remove(quest);

            if (quest.Model.QuestType == QuestType.Temporary) return;

	        switch (quest.Status)
	        {
                case QuestStatus.Completed:
                    _session.Quests.SetQuestCompleted(quest.Id, quest.StarId, true, _gameTime.TotalPlayTime);
                    break;
	            case QuestStatus.Failed:
                    _session.Quests.SetQuestCompleted(quest.Id, quest.StarId, false, _gameTime.TotalPlayTime);
                    break;
                case QuestStatus.Cancelled:
	                _session.Quests.CancelQuest(quest.Id, quest.StarId);
	                break;
                case QuestStatus.Error:
                default:
                    UnityEngine.Debug.LogException(new InvalidOperationException("QuestManager: Error has occured - " + quest.Model.Name));
                    _session.Quests.CancelQuest(quest.Id, quest.StarId);
                    break;
            }

            _questListChangedTrigger.Fire();
        }

        private void OnQuestEvent(IQuestEventData data)
	    {
            var time = _gameTime.TotalPlayTime;

            if (data.Type == QuestEventType.BeaconActivated)
	        {
	            var eventData = (BeaconEventData)data;
                _beaconQuests.UpdateQuests(eventData.StarId, eventData.Seed, time, _session);
	            Add(_beaconQuests.CreateRandomWeighted(_factory, eventData.Seed));
                return;
	        }
	        if (data.Type == QuestEventType.LocalEncounter)
	        {
	            var eventData = (LocalEncounterEventData)data;
                _localEncounterQuests.UpdateQuests(eventData.StarId, eventData.Seed, time, _session);
	            Add(_localEncounterQuests.CreateRandomWeighted(_factory, eventData.Seed));
                return;
	        }
	        if (data.Type == QuestEventType.NewStarSystemExplored)
	        {
	            var eventData = (StarEventData)data;
                var seed = _session.Game.Seed + eventData.StarId;
                _newStarExploredQuests.UpdateQuests(eventData.StarId, seed, time, _session);
	            Add(_newStarExploredQuests.CreateRandomWeighted(_factory, seed));
                return;
	        }
            if (data.Type == QuestEventType.FactionMissionAccepted)
	        {
	            var eventData = (StarEventData)data;
	            var seed = _session.Game.Seed + eventData.StarId + _session.Quests.GetFactionRelations(eventData.StarId);
                _factionQuests.UpdateQuests(eventData.StarId, seed, time, _session);
                Add(_factionQuests.CreateRandomWeighted(_factory, seed));
                return;
	        }

	        if (data.Type == QuestEventType.ArrivedAtStarSystem)
	        {
	            var eventData = (StarEventData)data;
	            var seed = _session.Game.Seed + eventData.StarId + _session.Quests.TotalQuestCount();
                _arrivedAtStarQuests.UpdateQuests(eventData.StarId, seed, time, _session);
	            Add(_arrivedAtStarQuests.CreateRandomWeighted(_factory, seed));
	        }

            if (data.Type == QuestEventType.Timer)
            {
                var seed = _session.Game.Seed + (int)(time / TimeSpan.TicksPerMinute) + _session.Quests.TotalQuestCount();
                _dailyQuests.UpdateQuests(_session.StarMap.PlayerPosition, seed, time, _session);
                Add(_dailyQuests.CreateFirstAvailable(_factory));
            }

			ProcessQuestEvent(data);
	    }

	    private void ProcessQuestEvent(IQuestEventData data)
	    {
	        foreach (var quest in _quests)
	            if (quest.TryProcessEvent(data) && !_recentlyUpdatedQuests.Contains(quest))
	                _recentlyUpdatedQuests.Add(quest);
        }

        private Quest _activeQuest;
	    private readonly List<Quest> _recentlyUpdatedQuests = new List<Quest>();
        private readonly List<Quest> _quests = new List<Quest>();
	    private readonly QuestCollection _beaconQuests = new QuestCollection();
	    private readonly QuestCollection _localEncounterQuests = new QuestCollection();
        private readonly QuestCollection _newStarExploredQuests = new QuestCollection();
	    private readonly QuestCollection _arrivedAtStarQuests = new QuestCollection();
        private readonly QuestCollection _factionQuests = new QuestCollection();
        private readonly QuestCollection _dailyQuests = new QuestCollection();

		private HashSet<int> _questBeacons = new HashSet<int>();
	    private HashSet<int> _questBeaconsOld = new HashSet<int>();

        private long _lastUpdateTime;
        private readonly ISessionData _session;
	    private readonly IDatabase _database;

        private readonly QuestListChangedSignal.Trigger _questListChangedTrigger;
	    private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        private readonly QuestActionRequiredSignal.Trigger _questActionRequiredTrigger;
	    private readonly QuestEventSignal _questEventSignal;

        private const long UpdateCooldown = 10 * TimeSpan.TicksPerSecond;

        private class QuestCollection
        {
            public void Assign(IEnumerable<QuestModel> quests, RequirementsFactory factory, int seed)
            {
                _quests.Clear();
                _quests.AddRange(quests.Select(item => new QuestInfo
                {
					QuestGiver = factory.CreateQuestGiver(item.Origin),
                    Requirements = factory.CreateForQuest(item.Requirement, seed + item.Id.Value),
                    Quest = item
                }));
            }

            public void Clear()
            {
                _quests.Clear();
            }

            public Quest CreateFirstAvailable(QuestFactory questFactory)
            {
                if (_allowedQuests.Count == 0) return null;
                var item = _allowedQuests[0];

                return questFactory.Create(item.Quest, item.StarId, item.Seed);
            }

            public Quest CreateRandomWeighted(QuestFactory questFactory, int seed)
            {
				var random = new System.Random(seed);
                var value = random.NextFloat() * (_totalWeight < 1f ? 1f : _totalWeight);

                foreach (var item in _allowedQuests)
                {
                    value -= item.Quest.Weight;
                    if (value > 0.0001f) continue;
                    return questFactory.Create(item.Quest, item.StarId, item.Seed);
                }

                return null;
            }

            public void UpdateQuests(int currentStarId, int seed, long currentTime, ISessionData session)
            {
                _allowedQuests.Clear();
                _totalWeight = 0f;
                var maxLevel = StarLayout.GetStarLevel(currentStarId, session.Game.Seed);
                var random = new System.Random(seed);

                foreach (var item in _quests)
                {
                    var quest = item.Quest;
                    //if (quest.Weight <= 0) continue;
                    if (quest.Level > maxLevel) continue;
                    if (quest.IsOnCooldown(session.Quests, currentTime, random)) continue;

                    var starId = item.QuestGiver.GetStartSystem(currentStarId, seed);
                    if (starId < 0) continue;

                    if (!quest.CanBeStarted(session.Quests, starId)) continue;
                    if (!item.Requirements.CanStart(starId, seed)) continue;

                    _allowedQuests.Add(new AllowedQuest { Quest = quest, StarId = starId, Seed = seed });
                    _totalWeight += quest.Weight;
                }
            }

            private float _totalWeight;
            private readonly List<QuestInfo> _quests = new List<QuestInfo>();
            private readonly List<AllowedQuest> _allowedQuests = new List<AllowedQuest>();

			private struct AllowedQuest
            {
                public QuestModel Quest;
                public int StarId;
                public int Seed;
            }

			private struct QuestInfo
            {
                public Domain.Quests.QuestGiver QuestGiver;
                public IQuestRequirements Requirements;
                public QuestModel Quest;
            }
        }
	}

	public class QuestActionRequiredSignal : SmartWeakSignal
	{
		public class Trigger : TriggerBase {}
	}

    public class QuestListChangedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
