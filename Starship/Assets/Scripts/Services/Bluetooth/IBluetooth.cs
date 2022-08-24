/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using leithidev.unityassets.nativebt.android.entities;

namespace Services.Bluetooth
{
    public enum AdapterState
    {
        Idle,
        Connecting,
        Connected,
        UnableToConnect,
    }

    public interface IBluetooth : Plugins.IMessageSender
    {
        bool IsAvailable { get; }
        bool Enabled { get; set; }
        bool Discovering { get; set; }
        void EnsureDiscoverable();
        void Connect(LWBluetoothDevice device);
        void Listen();
        AdapterState State { get; }
        IEnumerable<LWBluetoothDevice> Devices { get; }

        Plugins.INetworkGameObserver Observer { get; set; }
    }
}
*/