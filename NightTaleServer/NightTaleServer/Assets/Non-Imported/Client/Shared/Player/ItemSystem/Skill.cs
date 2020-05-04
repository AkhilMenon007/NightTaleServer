using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace FYP.Shared
{
    public class Skill : ScriptableObject
    {
        public SkillData skillData = null;


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(SkillLookup).FullName);
            if (guids.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var lookup = AssetDatabase.LoadAssetAtPath<SkillLookup>(path);
                if (skillData != null)
                {
                    var oldItem = skillData;
                    skillData = null;
                    var currItem = lookup.GetSkill(oldItem.skillID);
                    //Value in lookup is set to this and hasnt changed
                    if (currItem != this)
                    {
                        if (currItem != null)
                        {
                            Debug.LogError($"Another item already uses the item data with name {currItem.name}", currItem);
                            skillData = null;
                        }
                        else
                        {
                            skillData = oldItem;
                        }
                    }
                    else
                    {
                        skillData = oldItem;
                    }
                    lookup.RefreshSkillList();
                }
            }
            else if (guids.Length == 0)
            {
                Debug.LogError("ItemLookup not set");
            }
            else
            {
                Debug.LogError("Multiple ItemLookups found!!");
                foreach (var item in guids)
                {
                    Debug.Log(item);
                }
            }
        }
#endif
    }
}