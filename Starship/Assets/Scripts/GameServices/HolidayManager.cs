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
                if (date.Month ==4||date.Month ==3)
                {
                    int a=date.Year%19,b=date.Year/100,c=date.Year%100,d=b/4,e=b%4,f=(b+8)/25,g=(b-f+1)/3,h=(19*a+b-d-g+15)%30,i=c/4,k=c%4,l=(32+2*e+2*i-h-k)%7,m=(a+11*h+22*l)/451,month=(h+l-7*m+114)/31,day=((h+l-7*m+114)%31)+1;//Meeus/Jones/Butcher演算法（公历）
                    return date.Month == month && date.Day>=day && date.Day-day<=3||date.Month == month+1&& date.Day-day>=29;
                }
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
