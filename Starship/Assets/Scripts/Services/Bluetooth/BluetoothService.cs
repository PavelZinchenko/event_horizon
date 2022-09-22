/*using System.Collections.Generic;
using System.Linq;
using GameServices.Settings;
using UnityEngine;
using leithidev.unityassets.nativebt.android.core;
using leithidev.unityassets.nativebt.android.entities;
using Plugins;
using Services.Messenger;
using Zenject;

namespace Services.Bluetooth
{
    public class BluetoothService : IBluetooth
    {
#if UNITY_ANDROID //&& !UNITY_EDITOR

        [Inject]
        protected BluetoothService(IMessenger messenger)
        {
            _messenger = messenger;
            _btHandler = new AndroidNativeBTHandler();
            _btHandler.SetDelimeter(System.Environment.NewLine);

            //_btHandler.BTEventsHandler.BTAdapterDiscoveryCanceled += OnBTAdapterDiscoveryCanceled;
            _btHandler.BTEventsHandler.BTAdapterDiscoveryFinished += OnBTAdapterDiscoveryFinished;
            //_btHandler.BTEventsHandler.BTAdapterDiscoveryStarted += OnBTAdapterDiscoveryStarted;
            //_btHandler.BTEventsHandler.BTBondStateChanged += OnBTBondStateChanged;
            _btHandler.BTEventsHandler.BTDeviceFound += OnBTDeviceFound;

            _btHandler.BTEventsHandler.BTDeviceConnected += OnBtDeviceConnected;
            _btHandler.BTEventsHandler.BTDeviceConnecting += OnBTDeviceConnecting;
            _btHandler.BTEventsHandler.BTDeviceDisconnected += OnBtDeviceDisconnected;
            _btHandler.BTEventsHandler.BTDeviceConnectingFailed += OnBTDeviceConnectingFailed;
            _btHandler.BTEventsHandler.BTMessageReceived += OnMessageReceived;
            _btHandler.BTEventsHandler.BTMessageSent += OnMessageSent;
            _btHandler.BTEventsHandler.BTDeviceListening += OnBTDeviceListening;

            //_btHandler.BTEventsHandler.BTDisabled += OnBTDisabled;
            //_btHandler.BTEventsHandler.BTEnableApproved += OnBTEnableApproved;
            //_btHandler.BTEventsHandler.BTEnableCanceled += OnBTEnableCancelled;
            //_btHandler.BTEventsHandler.BTEnabled += OnBTEnabled;
            //_btHandler.BTEventsHandler.BTPaired += OnBTPaired;
            //_btHandler.BTEventsHandler.BTPairingFailed += OnBTPairingFailed;
            //_btHandler.BTEventsHandler.BTPairingRequest += OnBTPairingRequest;

            Debug.Log("BluetoothService constructor");
        }

        public bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public bool Enabled
        {
            get
            {
                return IsAvailable && _btHandler.BTWrapper.GetBTAdapter().IsEnabled();
            }
            set
            {
                Debug.Log("BluetoothService.SetEnabled(" + value + ")");

                if (!IsAvailable || value == Enabled)
                    return;

                if (value)
                    _btHandler.BTWrapper.ShowBTEnableRequest();
                else
                    _btHandler.BTWrapper.Disable();
            }
        }

        public Plugins.INetworkGameObserver Observer { get; set; }

        public AdapterState State
        {
            get { return _state; }
            private set
            {
                OptimizedDebug.Log("BT: Adapter state changed - " + value);
                _state = value;
                //_messenger.Broadcast<AdapterState>(EventType.BluetoothAdapterStateChanged, _state);

                //if (_state == AdapterState.Idle || _state == AdapterState.UnableToConnect)
                //    Listen();
            }
        }

        public void SendMessage(byte[] data, bool reliable, int offset, int length)
        {
            OptimizedDebug.Log("BT: SendMessage");

            if (State != AdapterState.Connected)
                return;

            _btHandler.BTWrapper.Send(System.Convert.ToBase64String(data, offset, length) + System.Environment.NewLine);
        }

        public void Disconnect()
        {
            OptimizedDebug.Log("BT: SendMessage");

            if (State != AdapterState.Connected)
                return;

            _btHandler.BTWrapper.Disconnect();
        }

        public void EnsureDiscoverable()
        {
            if (!IsAvailable)
                return;

            _btHandler.BTWrapper.ShowBTDiscoverRequest(600);
        }

        public bool Discovering
        {
            get { return IsAvailable && _btHandler.BTWrapper.GetBTAdapter().IsDiscovering(); }
            set
            {
                Debug.Log("BluetoothService.SetDiscovering(" + value + ")");

                if (!IsAvailable || Discovering == value)
                    return;

                if (value)
                {
                    _devices.Clear();
                    _btHandler.BTWrapper.StartDiscoverDevices();
                }
                else
                {
                    _btHandler.BTWrapper.CancelDiscoverDevices();
                }
            }
        }

        public IEnumerable<LWBluetoothDevice> Devices
        {
            get
            {
                var devices = new HashSet<LWBluetoothDevice>(_devices);

                if (Enabled)
                    foreach (var device in _btHandler.BTWrapper.GetPairedDevices())
                        devices.Add(device);

                return devices;
            }
        }

        public void Connect(LWBluetoothDevice device)
        {
            Debug.Log("BluetoothService.Connect(" + device.GetName() + ")");

            if (!IsAvailable || !Enabled)
                return;

            _btHandler.BTWrapper.Disconnect();
            _btHandler.BTWrapper.Connect(device, UUID);
        }

        public void Listen()
        {
            Debug.Log("BluetoothService.Listen");

            if (!IsAvailable || !Enabled)
                return;

            _btHandler.BTWrapper.Listen(true, UUID);
        }

        //private void OnBTEnableApproved()
        //{
        //    OptimizedDebug.Log("OnBTEnableApproved");
        //}

        //private void OnBTEnableCancelled()
        //{
        //    OptimizedDebug.Log("OnBTEnableCancelled");
        //}

        //private void OnBTEnabled()
        //{
        //    OptimizedDebug.Log("OnBTEnabled");
        //}

        //private void OnBTDisabled()
        //{
        //    OptimizedDebug.Log("OnBTDisabled");
        //}

        //private void OnBTBondStateChanged(int currentState, int previousState)
        //{
        //    OptimizedDebug.Log("OnBTBondStateChanged: " + previousState + " -> " + currentState);
        //}

        private void OnMessageSent(string msg)
        {
            OptimizedDebug.Log("BT: message sent - " + msg);
        }

        private void OnMessageReceived(string msg)
        {
            OptimizedDebug.Log("BT: message received - " + msg);

            if (Observer != null)
                Observer.OnMessage(System.Convert.FromBase64String(msg));
        }

        private void OnBTDeviceConnectingFailed(LWBluetoothDevice device)
        {
            OptimizedDebug.Log("OnBTDeviceConnectingFailed");
            State = AdapterState.UnableToConnect;
        }

        private void OnBTDeviceConnecting(LWBluetoothDevice device)
        {
            OptimizedDebug.Log("OnBTDeviceConnecting");
            State = AdapterState.Connecting;
        }

        private void OnBtDeviceConnected(LWBluetoothDevice device)
        {
            OptimizedDebug.Log("OnBtDeviceConnected");
            State = AdapterState.Connected;

            if (Observer != null)
                Observer.OnConnected();
        }

        private void OnBtDeviceDisconnected(LWBluetoothDevice device)
        {
            OptimizedDebug.Log("OnBtDeviceDisconnected");
            State = AdapterState.Idle;

            if (Observer != null)
                Observer.OnDisconnected();
        }

        //private void OnBTAdapterDiscoveryCanceled(IList<LWBluetoothDevice> devices)
        //{
        //    OptimizedDebug.Log("OnBTAdapterDiscoveryCanceled: " + devices.Count);
        //}

        private void OnBTAdapterDiscoveryFinished(IList<LWBluetoothDevice> devices)
        {
            OptimizedDebug.Log("OnBTAdapterDiscoveryFinished: " + devices.Count);
        }

        //private void OnBTAdapterDiscoveryStarted()
        //{
        //    OptimizedDebug.Log("OnBTAdapterDiscoveryStarted");
        //    AssignDevices(null);
        //    _messenger.Broadcast<bool>(EventType.BluetoothDiscovering, true);
        //}

        private void OnBTDeviceListening(string id)
        {
            OptimizedDebug.Log("OnBTDeviceListening: " + id);
        }

        private void OnBTDeviceFound(LWBluetoothDevice device)
        {
            OptimizedDebug.Log("OnBTDeviceFound - " + device.GetName());
            _devices.Add(device);
            //_messenger.Broadcast(EventType.BluetoothDeviceListChanged);
        }

        //private void OnBTPaired(LWBluetoothDevice device)
        //{
        //    OptimizedDebug.Log("OnBTPaired - " + device.GetName());
        //}

        //private void OnBTPairingFailed(LWBluetoothDevice device)
        //{
        //    OptimizedDebug.Log("OnBTPairingFailed - " + device.GetName());
        //}

        //private void OnBTPairingRequest(LWBluetoothDevice device)
        //{
        //    OptimizedDebug.Log("OnBTPairingRequest - " + device.GetName());
        //}

        //private void AssignDevices(IEnumerable<LWBluetoothDevice> devices)
        //{
        //    _devices.Clear();

        //    if (devices != null)
        //        foreach (var device in devices)
        //            _devices.Add(device);

        //    _messenger.Broadcast(EventType.BluetoothDeviceListChanged);
        //}

        private HashSet<LWBluetoothDevice> _devices = new HashSet<LWBluetoothDevice>();
        private AdapterState _state = AdapterState.Idle;
        private readonly AndroidNativeBTHandler _btHandler;
#else
        [Inject]
        protected BluetoothService(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void SendMessage(byte[] data, bool reliable, int offset, int length) {}
        public void Disconnect() {}
        public bool IsAvailable { get { return false; } }
        public bool Enabled { get { return false; } set {} }
        public bool Discovering { get { return false; } set {} }
        public void EnsureDiscoverable() {}
        public void Connect(LWBluetoothDevice device) {}

        public AdapterState State { get { return AdapterState.Disabled; } }
        public IEnumerable<LWBluetoothDevice> Devices { get { return Enumerable.Empty<LWBluetoothDevice>(); } }
        public INetworkGameObserver Observer { get; set; }
        public void Initialize() {}
        public void Listen() {}
#endif

        private readonly IMessenger _messenger;
        private const string UUID = "87A2FFCE-28A4-447F-B07A-518348625186";
    }
}
*/
