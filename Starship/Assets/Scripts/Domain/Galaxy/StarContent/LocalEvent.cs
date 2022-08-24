using Domain.Quests;
using GameServices.Quests;
using GameServices.Random;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class LocalEvent
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly IQuestManager _questManager;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly QuestEventSignal.Trigger _questEventTrigger;

        public bool IsActive(int starId)
        {
            var completedTime = _session.Events.CompletedTime(starId);
            var activationTime = completedTime + (5 + completedTime % 25) * System.TimeSpan.TicksPerDay;
            return System.DateTime.UtcNow.Ticks > activationTime;
        }

        public void Start(int starId)
        {
            if (!IsActive(starId))
                throw new System.InvalidOperationException();

#if UNITY_EDITOR
            _questEventTrigger.Fire(QuestEventFactory.CreateBeaconEventContext(starId, new System.Random().Next()));
#else
            var seed = starId + (int)_session.Events.CompletedTime(starId) + _random.Seed;
            _questEventTrigger.Fire(QuestEventFactory.CreateBeaconEventContext(starId, seed));
            _session.Events.Complete(starId);
#endif
            _starContentChangedTrigger.Fire(starId);
        }

        public struct Facade
        {
            public Facade(LocalEvent localEvent, int starId)
            {
                _localEvent = localEvent;
                _starId = starId;
            }

            public bool IsActive { get { return _localEvent.IsActive(_starId); } }
            public void Start() { _localEvent.Start(_starId); }

            private readonly LocalEvent _localEvent;
            private readonly int _starId;
        }
    }
}
