using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class GameData : ISerializableData
	{
        [Inject]
		public GameData(byte[] buffer = null)
        {
            _seed = (int)System.DateTime.Now.Ticks;
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "game";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion => 4;

		public int Seed
		{
			get { return _seed; }
		}

        public void Regenerate()
        {
            _seed = (int)System.DateTime.Now.Ticks;
            IsChanged = true;
        }

		public long GameStartTime
		{
			get { return _gameStartTime; }
			set
			{
				if (_gameStartTime != 0)
					throw new InvalidOperationException();

				IsChanged = true;
				_gameStartTime = value; 
			}
		}

        public long TotalPlayTime
        {
            get => _totalPlayTime;
            set
            {
                IsChanged = true;
                _totalPlayTime = value;
            }
        }

        public long SupplyShipStartTime => _supplyShipStartTime;

        public void StartSupplyShip()
        { 
            IsChanged = true;
            _supplyShipStartTime = System.DateTime.UtcNow.Ticks;
        }

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;
			
			foreach (var value in BitConverter.GetBytes(Seed))
				yield return value;
			foreach (var value in BitConverter.GetBytes(GameStartTime))
				yield return value;

		    foreach (var value in Helpers.Serialize(_supplyShipStartTime))
		        yield return value;
            foreach (var value in Helpers.Serialize(_totalPlayTime))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
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
                OptimizedDebug.Log("GameData: incorrect data version - " + version);
                throw new ArgumentException();
            }

            _seed = Helpers.DeserializeInt(buffer, ref index);
            _gameStartTime = Helpers.DeserializeLong(buffer, ref index);

            OptimizedDebug.Log("Start time: " + _gameStartTime);

            _supplyShipStartTime = Helpers.DeserializeLong(buffer, ref index);
            _totalPlayTime = Helpers.DeserializeLong(buffer, ref index);

            IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version)
		{
			if (version == 1)
			{
				data = Upgrade_1_2(data).ToArray();
				version = 2;
			}

            if (version == 2)
            {
                data = Upgrade_2_3(data).ToArray();
                version = 3;
            }

            if (version == 3)
            {
                data = Upgrade_3_4(data).ToArray();
                version = 4;
            }

            return version == CurrentVersion;
		}

		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index); // version
			foreach (var value in Helpers.Serialize(2))
				yield return value;

			var seed = Helpers.DeserializeInt(buffer, ref index);
			foreach (var value in Helpers.Serialize(seed))
				yield return value;

			var gameStartTime = Helpers.DeserializeLong(buffer, ref index);
			foreach (var value in Helpers.Serialize(gameStartTime))
				yield return value;

			Helpers.DeserializeInt(buffer, ref index); // energy
			Helpers.DeserializeLong(buffer, ref index); // energyRechargeStartTime

			var tutorialCompleted = buffer[index++] != 0;
			yield return tutorialCompleted ? (byte)1 : (byte)0;

			var easyMode = buffer[index++] != 0;
			yield return easyMode ? (byte)1 : (byte)0;
		}

        private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index); // version
            foreach (var value in Helpers.Serialize(3))
                yield return value;

            var seed = Helpers.DeserializeInt(buffer, ref index);
            foreach (var value in Helpers.Serialize(seed))
                yield return value;

            var gameStartTime = Helpers.DeserializeLong(buffer, ref index);
            foreach (var value in Helpers.Serialize(gameStartTime))
                yield return value;

            var tutorialCompleted = buffer[index++] != 0;
            yield return tutorialCompleted ? (byte)1 : (byte)0;

            index++; // easyMode

            foreach (var value in Helpers.Serialize((long)0)) // supplyShipStartTime
                yield return value;
        }

        private static IEnumerable<byte> Upgrade_3_4(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index); // version
            foreach (var value in Helpers.Serialize(4))
                yield return value;

            var seed = Helpers.DeserializeInt(buffer, ref index);
            var gameStartTime = Helpers.DeserializeLong(buffer, ref index);
            index++; // tutorialCompleted
            var supplyShipStartTime = Helpers.DeserializeLong(buffer, ref index);

            foreach (var value in Helpers.Serialize(seed))
                yield return value;
            foreach (var value in Helpers.Serialize(gameStartTime))
                yield return value;
            foreach (var value in Helpers.Serialize(supplyShipStartTime))
                yield return value;
            foreach (var value in Helpers.Serialize(TimeSpan.TicksPerDay*30)) // totalPlayTime
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
        }

        private int _seed;
		private long _gameStartTime;
        private long _supplyShipStartTime;
        private long _totalPlayTime;
    }
}
