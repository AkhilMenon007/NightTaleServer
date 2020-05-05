using DarkRift;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class WeaponSlot : ItemSlot
    {
        public ServerWeapon equippedWeapon => equippedItem as ServerWeapon;
        public ServerSkill equippedSkill { get; private set; } = null;
        public Action<ServerSkill> OnSkillEquipped { get; set; } = null;
        public Action<ServerSkill> OnSkillUnEquipped { get; set; } = null;

        public int equippedSkillID => equippedSkill == null ? SkillData.NO_SKILL_INDEX : equippedSkill.skillData.skillID;


        private ServerSkill lastSentSkill = null;
        private bool skillDirty = false;

        protected override bool isDirty 
        { 
            get => base.isDirty || skillDirty;
        }
        public void EquipSkill(Skill skill) 
        {
            if (equippedSkill != null) 
            {
                OnSkillUnEquipped?.Invoke(equippedSkill);
            }
            equippedSkill = skill as ServerSkill;

            syncMessage.activeSkill.skillID = (ushort)equippedSkillID;

            if (equippedSkill != null) 
            {
                OnSkillEquipped?.Invoke(equippedSkill);
            }
            if (lastSentSkill != equippedSkill) 
            {
                skillDirty = true;
                RegisterListener();
            }

        }
        public override void WriteStateDataToWriter(DarkRiftWriter writer)
        {
            base.WriteStateDataToWriter(writer);
            writer.Write(new ServerUpdateTag(ServerUpdateTags.SkillUpdate));
            writer.Write(new SkillChangeMessage { skillInfo = new SkillSetMessage { skillID = (ushort)equippedSkillID }, slot = slot });
        }
        public override void WriteUpdateDataToWriter(DarkRiftWriter writer)
        {
            base.WriteUpdateDataToWriter(writer);
            if (skillDirty)
            {
                writer.Write(new ServerUpdateTag(ServerUpdateTags.SkillUpdate));
                writer.Write(new SkillChangeMessage { skillInfo = new SkillSetMessage { skillID = (ushort)equippedSkillID }, slot = slot });
            }
        }
        public override void ResetUpdateData()
        {
            base.ResetUpdateData();
            lastSentSkill = equippedSkill;
            skillDirty = false;
            UnregisterListener();
        }
    }
}