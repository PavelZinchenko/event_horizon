using System.Collections.Generic;
using GameModel.Skills;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ViewModel.Skills
{
    [ExecuteInEditMode]
    public class SkillListUpdater : ObjectList
    {
        [SerializeField] private int _step = 32;

#if UNITY_EDITOR
        protected override void OnTransformChildrenChanged()
        {
            if (Application.isPlaying)
                return;

            base.OnTransformChildrenChanged();
            
            UnityEngine.Debug.Log("OnDataChanged");

            ValidateTree();
            //var prefab = Resources.Load<SkillInfoList>("Prefabs/Skills/SkillInfoList");

            var skills = new List<GameServices.Database.Skills.SkillInfo>();
            for (var i = 0; i < Children.Length; ++i)
            {
                var child = Children[i];
                if (child == null)
                    continue;
                var item = child.GetComponent<SkillTreeNode>();
                if (item == null)
                    continue;

                skills.Add(new GameServices.Database.Skills.SkillInfo { Id = i, Multilpler = item.Multiplier, Type = item.Type });
            }

            var skillDb = new GameServices.Database.Skills();
            skillDb.Assign(skills);

            //EditorUtility.SetDirty(prefab);
        }

        private void ValidateTree()
        {
            foreach (Transform child in transform)
            {
                var skill = child.GetComponent<SkillTreeNode>();
                if (skill == null)
                    continue;

                skill.ValidateLinks();

                var position = child.localPosition;
                position.x = Mathf.Round(position.x/_step)*_step;
                position.y = Mathf.Round(position.y/_step)*_step;
                child.transform.localPosition = position;
            }
        }
#endif
    }
}
