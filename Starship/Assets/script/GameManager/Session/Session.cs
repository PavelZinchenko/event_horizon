using System.Linq;
using GameModel.GameData;

namespace GameModel.Session
{
//    public class Session : ISession
//    {
//        public Session(GameDataBase data)
//        {
//            //_gameData = data;
//            //_totalTime = _gameData.TimePlayed;
//            //_currentStar = new Cached<Star, int>(id => StarMap[id]);
//        }

//        public void Start()
//        {
//            //StarMap.Initialize();
//            //GameLogic.Initialize();
//            //QuestLog.Initialize();
//			//Player.Initialize();
//            //Achievements.Initialize();
//        }
        
//        public void Update(float elapsedTime)
//        {
//            _totalTime += (long)(elapsedTime*System.TimeSpan.TicksPerSecond);
//            _gameLogic.Update(elapsedTime);
//        }

//        public GameDataBase GameData 
//        {
//            get
//            {
//                //_gameData.TimePlayed = _totalTime;
//                return _gameData;
//            }
//        }

//        //public Player Player { get { return _player /*?? (_player = new Player())*/; } }
//        //public RewardGenerator RewardGenerator { get { return _rewardGenerator ?? (_rewardGenerator = new RewardGenerator()); } }
//        //public StarMap StarMap { get { return _starMap ?? (_starMap = new StarMap()); } }
//        public RegionMap RegionMap { get { return _regionMap /*?? (_regionMap = new RegionMap())*/; } }
//        //public QuestLog QuestLog { get { return _questLog ?? (_questLog = new QuestLog()); } }
//        //public GameLogic GameLogic { get { return _gameLogic ?? (_gameLogic = new GameLogic()); } }
//        //public Research Research {  get { return _research ?? (_research = new Research()); } }
//        public Achievements.Manager Achievements { get { return _achievements ?? (_achievements = new Achievements.Manager()); } }

//        public Star CurrentStar { get { return _currentStar.GetValue(/*GameLogic.PlayerPosition*/0); } }

//        public long TotalTime { get { return _totalTime; } }

////        public static void ResetData(System.Action action = null)
////        {
////            GameEvents.Instance.Cleanup();
////            //Game.Session.GameData.Reset();
////            if (action != null) action.Invoke();
////            Instance.Reset();
////        }

//        public int RandomInt(int index)
//        {
//            if (_randomNumbers == null)
//                GenerateRandomNumbers();

//            if (index < 0)
//                index = -index;

//            return index < RandomNumbersCount ? _randomNumbers[index] : _randomNumbers[index % RandomNumbersCount];
//        }       

//        public int RandomInt(int index, int maxValue)
//        {
//            return RandomInt(index) % (maxValue + 1);
//        }       

//        public int RandomInt(int index, int minValue, int maxValue)
//        {
//            return minValue + RandomInt(index) % (maxValue - minValue + 1);
//        }       

//        private void GenerateRandomNumbers()
//        {
//            var seed = _gameData.Game.Seed;
//            var random = new System.Random(seed);
//            _randomNumbers = Enumerable.Range(0, RandomNumbersCount).OrderBy(value => random.Next()).ToArray();
//        }

//        private readonly GameDataBase _gameData;
//        //private StarMap _starMap;
//        private RegionMap _regionMap;
//        private GameLogic _gameLogic;
//        //private QuestLog _questLog;
//        //private Research _research;
//        private Achievements.Manager _achievements;
//        private Cached<Star, int> _currentStar;

//        private long _totalTime;
//        private int[] _randomNumbers;
//        private const int RandomNumbersCount = 10000;
//    }
}
