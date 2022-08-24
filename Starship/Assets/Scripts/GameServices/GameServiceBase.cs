using Session;
using Zenject;

namespace GameServices
{
    public abstract class GameServiceBase : IInitializable
    {
        [Inject]
        protected GameServiceBase(SessionDataLoadedSignal dataLoadedSignal, SessionCreatedSignal sessionCreatedSignal)
        {
            _dataLoadedSignal = dataLoadedSignal;
            _dataLoadedSignal.Event += OnSessionDataLoaded;
            _sessionCreatedSignal = sessionCreatedSignal;
            _sessionCreatedSignal.Event += OnSessionCreated;
        }

        public virtual void Initialize()
        {
        //    OnSessionDataLoaded();
        //    OnSessionCreated();
        }

        /// <remarks>game services not yet initialized</remarks>
        protected abstract void OnSessionDataLoaded();
        /// <remarks>game services available here</remarks>
        protected abstract void OnSessionCreated();

        private readonly SessionDataLoadedSignal _dataLoadedSignal;
        private readonly SessionCreatedSignal _sessionCreatedSignal;
    }
}
