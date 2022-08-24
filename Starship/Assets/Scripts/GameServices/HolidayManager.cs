using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices;
using GameServices.Quests;
using Services.InternetTime;
using Session;

namespace Game
{
    public class HolidayManager : GameServiceBase
    {
        public HolidayManager(
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal,
            InternetTimeService timeService, 
            IQuestManager questManager, 
            IDatabase database) 
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _timeService = timeService;
            _questManager = questManager;
            _database = database;
        }

        public bool IsChristmas
        {
            get
            {
//#if UNITY_EDITOR
//                return true;
//#endif
                var date = _timeService.DateTime;
                return date.Month == 12 && date.Day >= 10 || date.Month == 1 && date.Day <= 15;
            }
        }

        public bool IsEaster
        {
            get
            {
                //#if UNITY_EDITOR
                //                return true;
                //#endif
                var date = _timeService.DateTime;
                if (date.Year == 2019)
                    return date.Month == 4 && date.Day >= 19;

                return false;
            }
        }

        protected override void OnSessionDataLoaded()
        {
        }

        protected override void OnSessionCreated()
        {
            if (IsEaster) _questManager.StartQuest(_database.GetQuest(new ItemId<QuestModel>(10)));
        }

        private readonly InternetTimeService _timeService;
        private readonly IQuestManager _questManager;
        private readonly IDatabase _database;
    }
}
