using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace FYP.Shared
{
    [CreateAssetMenu(menuName ="FYP/SkillSystem/SkillLookup")]
    public class SkillLookup : ScriptableObject
    {

        [SerializeField]
        [ReorderableList]
        private List<SkillDataSkillPair> skillList = new List<SkillDataSkillPair>();

        [SerializeField]
        [ReadOnly]
        private int skillDataCount = 0;


        public Skill GetSkill(uint id)
        {
            int index = (int)id - SkillData.RESERVED_SKILL_COUNT;
            if (index >= 0 && index < skillList.Count)
            {
                return skillList[index].skill;
            }
            return null;
        }
        private void OnValidate()
        {
            foreach (var obj in skillList)
            {
                if (obj != null && obj.skill != null && obj.skillData != null)
                {
                    obj.skill.skillData = obj.skillData;
                }
            }
        }

#if UNITY_EDITOR

        [ContextMenu("RefreshSkillList")]
        public void RefreshSkillList()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(Skill).FullName);
            var skillGuids = AssetDatabase.FindAssets("t:" + typeof(SkillData).FullName);
            skillDataCount = skillGuids.Length;
            skillList = new List<SkillDataSkillPair>(skillDataCount);

            for (int i = 0; i < skillDataCount; i++)
            {
                skillList.Add(new SkillDataSkillPair());
            }
            foreach (var item in skillGuids)
            {
                var wData = AssetDatabase.LoadAssetAtPath<SkillData>(AssetDatabase.GUIDToAssetPath(item));
                skillList[wData.GetIndex()].skillData = wData;
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var data = AssetDatabase.LoadAssetAtPath<Skill>(path);
                if (data != null && data.skillData != null)
                {
                    skillList[data.skillData.GetIndex()].skill = data;
                }
            }
        }
#endif


    }
    [System.Serializable]
    public class SkillDataSkillPair
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "<Pending>")]
        public Skill skill = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "<Pending>")]
        public SkillData skillData = null;
        public SkillDataSkillPair()
        {
            skill = null;
            skillData = null;
        }
    }
}