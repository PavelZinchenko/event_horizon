using System;
using System.Collections.Generic;
using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using GameModel.Skills;
using ModestTree;
using Session;
using UnityEngine;
using Zenject;

namespace GameServices.Database
{
    public class Skills
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly IDatabase _database;

        [Inject]
        public Skills()
        {
            var textAsset = Resources.Load<TextAsset>(AssetPath);
            if (textAsset == null)
                return;

            var data = JsonUtility.FromJson<SkillInfoList>(textAsset.text);
            foreach (var skill in data.Skills)
            {
                _skills.Add(skill.Id, skill);

                if (skill.Type.IsCommonSkill())
                    TotalSkills++;
            }
        }

        public SkillInfo this[int id]
        {
            get
            {
                SkillInfo skillInfo;
                return _skills.TryGetValue(id, out skillInfo) ? skillInfo : SkillInfo.Empty;
            }
        }

        public int TotalSkills { get; private set; }

        public bool IsFree(SkillType type)
        {
            if (type == SkillType.Undefined)
                return true;

            if (type == SkillType.RequierementMaxLevel)
            {
                return _session.Fleet.Ships.Any(item => item.Experience >= Maths.Experience.MaxPlayerExperience);
            }

            if (type == SkillType.RequierementBeatAllEnemies)
            {
                var factions = _database.FactionList.Where(item => !item.Hidden && item != Faction.Neutral).Select(item => item.Id.Value).ToHashSet();

                foreach (var faction in _session.Regions.GetCapturedFactions())
                    factions.Remove(faction.Value);

                return factions.Count == 0;
            }

            return false;
        }

#if UNITY_EDITOR
        public void Assign(IEnumerable<SkillInfo> skills)
        {
            _skills.Clear();
            foreach (var skill in skills)
                _skills.Add(skill.Id, skill);

            SaveDatabase();
        }

        private void SaveDatabase()
        {
            var skills = new SkillInfoList { Skills = _skills.Values.ToList() };
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/" + AssetPath + ".json", JsonUtility.ToJson(skills, true));
        }
#endif

        private readonly Dictionary<int, SkillInfo> _skills = new Dictionary<int, SkillInfo>();
        private readonly string AssetPath = "skills";

        [Serializable]
        public struct SkillInfo
        {
            public int Id;
            public GameModel.Skills.SkillType Type;
            public int Multilpler;

            public bool IsEmpty { get { return Id < 0; } }
            public static readonly SkillInfo Empty = new SkillInfo { Id = -1 };
        }

        [Serializable]
        public struct SkillInfoList
        {
            public List<SkillInfo> Skills;
        }
    }
}
