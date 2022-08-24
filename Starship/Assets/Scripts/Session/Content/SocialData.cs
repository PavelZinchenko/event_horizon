using System;
using System.Collections.Generic;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class SocialData : ISerializableData
    {
        [Inject]
        public SocialData(byte[] buffer = null)
        {
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "social";

        public bool IsChanged { get; private set; }
        public static int CurrentVersion { get { return 1; } }

        public int LastFacebookPostDate
        {
            get { return _lastFacebookPostDate; }
            set
            {
                IsChanged |= _lastFacebookPostDate != value;
                _lastFacebookPostDate = value;
            }
        }

        public int LastDailyRewardDate
        {
            get { return _lastDailyRewardDate; }
            set
            {
                IsChanged |= _lastDailyRewardDate != value;
                _lastDailyRewardDate = value;
            }
        }

        public int FirstDailyRewardDate
        {
            get { return _firstDailyRewardDate; }
            set
            {
                IsChanged |= _firstDailyRewardDate != value;
                _firstDailyRewardDate = value;
            }
        }

        public IEnumerable<byte> Serialize()
        {
            IsChanged = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;

            foreach (var value in Helpers.Serialize(LastFacebookPostDate))
                yield return value;
            foreach (var value in Helpers.Serialize(FirstDailyRewardDate))
                yield return value;
            foreach (var value in Helpers.Serialize(LastDailyRewardDate))
                yield return value;

            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
            foreach (var value in Helpers.Serialize(0))
                yield return value;
        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            int index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);
            if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
            {
                UnityEngine.Debug.Log("SocialData: incorrect data version - " + version);
                throw new ArgumentException();
            }

            _lastFacebookPostDate = Helpers.DeserializeInt(buffer, ref index);
            _firstDailyRewardDate = Helpers.DeserializeInt(buffer, ref index);
            _lastDailyRewardDate = Helpers.DeserializeInt(buffer, ref index);

            IsChanged = false;
        }

        private static bool TryUpgrade(ref byte[] data, int version)
        {
            return version == CurrentVersion;
        }

        private int _lastFacebookPostDate;
        private int _firstDailyRewardDate;
        private int _lastDailyRewardDate;
    }
}
