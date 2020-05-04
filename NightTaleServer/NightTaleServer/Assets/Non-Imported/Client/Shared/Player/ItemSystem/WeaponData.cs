using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace FYP.Shared
{
    [CreateAssetMenu(menuName = "FYP/GameData/Weapon")]
    public class WeaponData : ItemData
    {
        [ReorderableList]
        public List<SkillData> allowedSkills = new List<SkillData>();


    }
}