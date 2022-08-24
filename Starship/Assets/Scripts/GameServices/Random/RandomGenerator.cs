using System.Linq;
using Session;
using Zenject;

namespace GameServices.Random
{
    public sealed class RandomGenerator : GameServiceBase, IRandom
    {
        [Inject]
        public RandomGenerator(ISessionData session, SessionDataLoadedSignal dataLoadedSignal, SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _session = session;
        }

        public int Seed { get; private set; }

        public System.Random CreateRandom(int seed = 0)
        {
            return new System.Random(Seed + seed);
        }

        public int RandomInt(int index)
        {
            if (_randomNumbers == null)
                GenerateRandomNumbers();

            if (index < 0)
                index = -index;

            return index < RandomNumbersCount ? _randomNumbers[index] : _randomNumbers[index % RandomNumbersCount];
        }

        public int RandomInt(int index, int maxValue)
        {
            return RandomInt(index) % (maxValue + 1);
        }

        public int RandomInt(int index, int minValue, int maxValue)
        {
            return minValue + RandomInt(index) % (maxValue - minValue + 1);
        }

        protected override void OnSessionDataLoaded()
        {
            Seed = _session.Game.Seed;
            GenerateRandomNumbers();
        }

        protected override void OnSessionCreated()
        {
        }

        private void GenerateRandomNumbers()
        {
            var seed = Seed;
            var random = new System.Random(seed);
            _randomNumbers = Enumerable.Range(0, RandomNumbersCount).OrderBy(value => random.Next()).ToArray();
        }

        private int[] _randomNumbers;
        private readonly ISessionData _session;
        private const int RandomNumbersCount = 10000;
    }
}
