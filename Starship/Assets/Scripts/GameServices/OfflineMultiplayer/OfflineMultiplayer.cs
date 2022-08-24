using Session;
using Utils;
using Zenject;

namespace GameServices.Multiplayer
{
    public enum Status
    {
        Unknown,
        NotLoggedIn,
        ShipNotAllowed,
        Ready,
        Connecting,
        ConnectionError,
    }

    public class OfflineMultiplayer : GameServiceBase
    {
        [Inject]
        public OfflineMultiplayer(
            SessionDataLoadedSignal dataLoadedSignal,
            SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
        }

        public IPlayerInfo Player => null;

        public Status Status
        {
            get => Status.NotLoggedIn;
            set {}
        }

        public int AvailableBattles => 0;
        public long TimeToNextBattle => 0;

        public void BuyMoreTickets(int count = 0) {}

        public void UpdateStatus() {}

        public void FindOpponent() { }

        public void PrepareToFight(IPlayerInfo player) { }

        public void Fight(IPlayerInfo enemy) { }

        protected override void OnSessionDataLoaded() { }

        protected override void OnSessionCreated() { }
    }

    public class MultiplayerStatusChangedSignal : SmartWeakSignal<Status>
    {
        public class Trigger : TriggerBase { }
    }

    public class EnemyFleetLoadedSignal : SmartWeakSignal<IPlayerInfo>
    {
        public class Trigger : TriggerBase { }
    }

    public class EnemyFoundSignal : SmartWeakSignal<IPlayerInfo>
    {
        public class Trigger : TriggerBase { }
    }
}
