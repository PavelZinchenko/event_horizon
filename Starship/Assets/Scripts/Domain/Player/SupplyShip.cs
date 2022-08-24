using System;
using Economy;
using Session;
using Session.Content;
using Utils;
using Zenject;

namespace GameServices.Player
{
    public class SupplyShip : GameServiceBase, ITickable
    {
        public SupplyShip(
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal,
            FuelValueChangedSignal fuelValueChangedSignal,
            PlayerPositionChangedSignal playerPositionChanged,
            SupplyShipActivatedSignal.Trigger supplyShipActivatedTrigger,
            ISessionData session) 
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _supplyShipActivatedTrigger = supplyShipActivatedTrigger;
            _fuelValueChangedSignal = fuelValueChangedSignal;
            _fuelValueChangedSignal.Event += OnFuelValueChanged;
            _playerPositionChanged = playerPositionChanged;
            _playerPositionChanged.Event += OnPlayerPositionChanged;
            _session = session;
        }

        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly PlayerResources _resources;

        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                if (_isActive == value)
                    return;
                
                _isActive = value;
                _supplyShipActivatedTrigger.Fire(_isActive);
            }
        }

        public long WaitingTime
        {
            get
            {
                if (!IsActive)
                    return 0;

                if (_finishTime <= 0)
                    _finishTime = _session.Game.SupplyShipStartTime + OneLevelTime*(1L + _motherShip.CurrentStar.Level);

                return Math.Max(0, _finishTime - System.DateTime.UtcNow.Ticks);
            }
        }

        public Price SpeedUpPrice { get { return Price.Premium(1 + (int)(WaitingTime/OneLevelTime)/10); } }

        public void SpeedUp()
        {
            if (SpeedUpPrice.TryWithdraw(_resources))
                OnArrived();
        }

        public void Tick()
        {
            if (!IsActive || WaitingTime > 0)
                return;

            OnArrived();
        }

        protected override void OnSessionDataLoaded()
        {
            if (_session.Game.SupplyShipStartTime > System.DateTime.UtcNow.Ticks)
                _session.Resources.Fuel = 0;
            else
                IsActive = _session.Resources.Fuel <= 0;
        }

        protected override void OnSessionCreated()
        {
        }

        private void OnPlayerPositionChanged(int position)
        {
            _finishTime = 0;
        }

        private void OnArrived()
        {
            _resources.Fuel = MotherShip.FuelMinimum;
            _finishTime = 0;
        }

        private void OnFuelValueChanged(int value)
        {
            _finishTime = 0;

            if (_session.Resources.Fuel <= 0)
            {
                if (IsActive)
                    return;

                _session.Game.StartSupplyShip();
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }
        }

        private bool _isActive;
        private long _finishTime;
        private const long OneLevelTime = System.TimeSpan.TicksPerMinute*5;
        private readonly FuelValueChangedSignal _fuelValueChangedSignal;
        private readonly PlayerPositionChangedSignal _playerPositionChanged;
        private readonly SupplyShipActivatedSignal.Trigger _supplyShipActivatedTrigger;
        private readonly ISessionData _session;
    }

    public class SupplyShipActivatedSignal : SmartWeakSignal<bool>
    {
        public class Trigger : TriggerBase { }
    }
}
