using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FYP.Shared
{
    [CreateAssetMenu(menuName ="FYP/GameData/SkillData")]
    public class SkillData : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly]
        public ushort skillID = 0;
        public const int RESERVED_SKILL_COUNT = 1;
        public string skillName = "";
        public float cooldown = 0f;
        public float cost = 0f;

        public const int NO_SKILL_INDEX = 0;

        public int GetIndex() => skillID - RESERVED_SKILL_COUNT;

#if UNITY_EDITOR
        [ContextMenu("Reset IDs")]
        public void ResetID()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(SkillData).Name);
            SkillData[] a = new SkillData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            }

            for (ushort i = 0; i < a.Length; i++)
            {
                a[i].skillID = (ushort)(i + RESERVED_SKILL_COUNT);
            }
        }
#endif
    }
}