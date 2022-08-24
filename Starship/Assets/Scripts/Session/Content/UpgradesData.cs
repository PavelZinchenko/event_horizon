using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class UpgradesData : ISerializableData
	{
        [Inject]
		public UpgradesData(PlayerSkillsResetSignal.Trigger playerSkillsResetTrigger, byte[] buffer = null)
        {
            _playerSkillsResetTrigger = playerSkillsResetTrigger;

            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "upgrades";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 3; } }

        public long PlayerExperience
        {
            get { return _playerExperience; }
            set
            {
                IsChanged = true;
                _playerExperience = value;
            }
        }

        public void AddSkill(int id)
        {
            IsChanged = _skills.Add(id);
        }

        public bool HasSkill(int id)
        {
            return _skills.Contains(id);
        }

        public int ResetCounter
        {
            get { return _resetCounter; }
            private set
            {
                _resetCounter = Math.Min(value, 10);
                IsChanged = true;
            }
        }

        public void ResetSkills()
        {
            if (!_skills.Any())
                return;

            _skills.Clear();
            ResetCounter++;

            _playerSkillsResetTrigger.Fire();
        }

        public IEnumerable<int> Skills { get { return _skills; } }

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;
			
			foreach (var value in Helpers.Serialize(_skills))
				yield return value;
			foreach (var value in Helpers.Serialize((long)_playerExperience))
				yield return value;
            foreach (var value in Helpers.Serialize((int)_resetCounter))
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
            var resetRequired = false;
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version, out resetRequired))
			{
				UnityEngine.Debug.Log("UpgradesData: incorrect data version");
                throw new ArgumentException();
            }

			_skills = Helpers.DeserializeHashSet(buffer, ref index);
			_playerExperience = Helpers.DeserializeLong(buffer, ref index);

            if (resetRequired)
                _skills.Clear();

            if (index < buffer.Length)
                _resetCounter = Helpers.DeserializeInt(buffer, ref index);

            IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version, out bool resetRequired)
		{
		    resetRequired = false;

			if (version == 1)
			{
				data = Upgrade_1_2(data).ToArray();
				version = 2;
			}

		    if (version == 2)
		    {
		        resetRequired = true;
		        version = 3;
		    }

			return version == CurrentVersion;
		}

		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				

			foreach (var value in Helpers.Serialize(0)) // skills
				yield return value;

			var count = Helpers.DeserializeInt(buffer, ref index);
			var level = 0;
			for (int i = 0; i < count; ++i)
			{
				Helpers.DeserializeInt(buffer, ref index); // key
                var value = Helpers.DeserializeInt(buffer, ref index);
				level += value;
			}

			var experience = GameModel.Skills.Experience.FromLevel(level);
			foreach (var value in Helpers.Serialize((long)experience))
				yield return value;
		}

        private ObscuredInt _resetCounter = 0;
        private ObscuredLong _playerExperience = 0;
        private HashSet<int> _skills = new HashSet<int>();
        private readonly PlayerSkillsResetSignal.Trigger _playerSkillsResetTrigger;
	}

    public class PlayerSkillsResetSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
}
