using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace Services.Storage
{
    public enum CloudStorageStatus
    {
        NotReady,
        Idle,
        Synchronizing,
        Loading,
        Saving,
    }

    public interface ISavedGame
    {
        string Name { get; }
        DateTime ModificationTime { get; }

        void Save(IGameData data);
        void Load(IGameData data, string mod);
        void Delete();
    }

    public interface ICloudStorage
    {
        CloudStorageStatus Status { get; }
        IEnumerable<ISavedGame> Games { get; }

        void Synchronize();
        void Save(string filename, IGameData data);

        bool TryLoadFromCopy(IGameData data, string mod);

        string LastErrorMessage { get; }
    }

    public class CloudStorageStatusChangedSignal : SmartWeakSignal<CloudStorageStatus>
    {
        public class Trigger : TriggerBase { }
    }

    public class CloudLoadingCompletedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class CloudSavingCompletedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class CloudOperationFailedSignal : SmartWeakSignal<string>
    {
        public class Trigger : TriggerBase { }
    }

    public class CloudSavedGamesReceivedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
