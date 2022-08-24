using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using UnityEngine;
using Zenject;

namespace Session.Content
{
    public class PvpData : ISerializableData
    {
        [Inject]
        public PvpData(byte[] buffer = null)
        {
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "pvp";

        public bool IsChanged { get; private set; }
        public static int CurrentVersion { get { return 2; } }

        public long LastFightTime
        {
            get { return _arenaLastFightTime; }
            set
            {
                IsChanged = _arenaLastFightTime != value;
                _arenaLastFightTime = value;
            }
        }

        public long TimerStartTime
        {
            get { return _arenaTimerStartTime; }
            set
            {
                IsChanged = _arenaTimerStartTime != value;
                _arenaTimerStartTime = value;
            }
        }

        public int FightsFromTimerStart
        {
            get { return _arenaFightsFromTimerStart; }
            set
            {
                IsChanged = _arenaFightsFromTimerStart != value;
                _arenaFightsFromTimerStart = value;
            }
        }

        public IEnumerable<byte> Serialize()
        {
            IsChanged = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;
            foreach (var value in Helpers.Serialize(_arenaTimerStartTime))
                yield return value;
            foreach (var value in Helpers.Serialize(_arenaLastFightTime))
                yield return value;
            foreach (var value in Helpers.Serialize(_arenaFightsFromTimerStart))
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            yield break;
        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            int index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);
            if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
            {
                Debug.LogError("PvpData: incorrect data version");
                throw new ArgumentException();
            }

            _arenaTimerStartTime = Helpers.DeserializeLong(buffer, ref index);
            _arenaLastFightTime = Helpers.DeserializeLong(buffer, ref index);
            _arenaFightsFromTimerStart = Helpers.DeserializeInt(buffer, ref index);

            Helpers.DeserializeInt(buffer, ref index); // reserved
            Helpers.DeserializeInt(buffer, ref index); // reserved
            Helpers.DeserializeInt(buffer, ref index); // reserved
            Helpers.DeserializeInt(buffer, ref index); // reserved

            IsChanged = false;
        }

        private static bool TryUpgrade(ref byte[] data, int version)
        {
            if (version == 1)
            {
                data = Upgrade_1_2(data).ToArray();
                version = 2;
            }

            return version == CurrentVersion;
        }

        private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
        {
            UnityEngine.Debug.Log("PvpData.Upgrade_1_2");

            var index = 0;
            Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(2)) // version
                yield return value;

            foreach (var value in Helpers.Serialize(0L))
                yield return value;
            foreach (var value in Helpers.Serialize(0L))
                yield return value;
            for (var i = 0; i < 5; ++i)
                foreach (var value in Helpers.Serialize(0))
                    yield return value;
        }


        private int _arenaFightsFromTimerStart;
        private long _arenaLastFightTime;
        private long _arenaTimerStartTime;
    }
}
