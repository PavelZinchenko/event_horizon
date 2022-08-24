using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;
using Zenject;

namespace Session.Content
{
    public class AchievementData : ISerializableData
    {
        [Inject]
        public AchievementData(byte[] buffer = null)
        {
            IsChanged = true;

            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);
        }

        public string FileName { get { return Name; } }
        public const string Name = "achievements";

        public bool IsChanged { get; private set; }

        public static int CurrentVersion { get { return 3; } }

        public bool IsUnlocked(GameModel.Achievements.AchievementType type)
        {
            return _unlockedAchievements.Contains((int)type);
        }

        public void UnlockAchievement(GameModel.Achievements.AchievementType type)
        {
            IsChanged |= _unlockedAchievements.Add((int)type);
        }

        public IEnumerable<byte> Serialize()
        {
            IsChanged = false;

            foreach (var value in Helpers.Serialize(CurrentVersion))
                yield return value;

            foreach (var value in Helpers.Serialize(_unlockedAchievements))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
        }

        private void Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                throw new ArgumentException();

            var index = 0;
            var version = Helpers.DeserializeInt(buffer, ref index);
            if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
            {
                UnityEngine.Debug.Log("AchievementData: incorrect data version");
                throw new ArgumentException();
            }

            _unlockedAchievements = Helpers.DeserializeHashSet(buffer, ref index);
            IsChanged = false;
        }

        private static bool TryUpgrade(ref byte[] data, int version)
        {
            if (version < 3)
            {
                data = Upgrade_3().ToArray();
                version = 3;
            }

            return version == CurrentVersion;
        }

        private static IEnumerable<byte> Upgrade_3()
        {
            UnityEngine.Debug.Log("AchievementData.Upgrade_3");

            var version = 3;
            foreach (var value in Helpers.Serialize(version))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // _unlockedAchievements
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;
        }

        private HashSet<int> _unlockedAchievements = new HashSet<int>();
    }
}
