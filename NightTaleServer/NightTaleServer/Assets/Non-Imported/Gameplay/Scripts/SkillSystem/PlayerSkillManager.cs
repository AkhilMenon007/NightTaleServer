using DarkRift;
using DarkRift.Server;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class PlayerSkillManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerEntity playerEntity = null;
        private ServerPlayer serverPlayer = null;

        [SerializeField]
        private SkillLookup skillLookup = null;
        [SerializeField]
        private ItemLookup itemLookup = null;

        public WeaponSlot primaryHand = null;
        public WeaponSlot secondaryHand = null;
        public ArmorSlot chest = null;
        public ArmorSlot arms = null;
        public ArmorSlot legs = null;



        public WeaponSlot GetWeaponSlot(EquipSlot slot)
        {
            switch (slot)
            {
                case EquipSlot.PrimaryHand:
                    {
                        return primaryHand;
                    }
                case EquipSlot.SecondaryHand:
                    {
                        return secondaryHand;
                    }
            }
            return null;
        }
        public ArmorSlot GetArmorSlot(EquipSlot slot)
        {
            switch (slot)
            {
                case EquipSlot.Arms:
                    {
                        return arms;
                    }
                case EquipSlot.Legs:
                    {
                        return legs;
                    }
                case EquipSlot.Chest:
                    {
                        return chest;
                    }
            }
            return null;
        }

        public void GetEquipData(List<EquipMessage> equipList)
        {
            equipList.Clear();
            AddWeaponData(equipList, primaryHand);
            AddWeaponData(equipList, secondaryHand);
            AddArmorData(equipList, chest);
            AddArmorData(equipList, legs);
            AddArmorData(equipList, arms);
        }

        public void AddWeaponData(List<EquipMessage> equipList,WeaponSlot slot) 
        {
            if (slot.equippedItem != null)
            {
                if (slot.equippedSkill != null)
                {
                    equipList.Add(new EquipMessage
                    {
                        slotAndID = new EquipRequest { equipSlot = slot.slot, itemID = slot.equippedItem.itemData.id }
                        ,
                        activeSkill = new SkillSetMessage { skillID = slot.equippedSkill.skillData.skillID }
                    });
                }
                else
                {
                    equipList.Add(new EquipMessage
                    {
                        slotAndID = new EquipRequest { equipSlot = slot.slot, itemID = slot.equippedItem.itemData.id }
                        ,
                        activeSkill = new SkillSetMessage { skillID = SkillData.NO_SKILL_INDEX }
                    });
                }
            }
        }
        public void AddArmorData(List<EquipMessage> equipList,ArmorSlot slot) 
        {
            if (slot.equippedItem != null)
            {
                equipList.Add(new EquipMessage
                {
                    slotAndID = new EquipRequest { equipSlot = slot.slot, itemID = slot.equippedItem.itemData.id }
                });
            }
        }
        public ItemSlot GetItemSlot(EquipSlot slot)
        {
            var weapSlot = GetWeaponSlot(slot);
            if (weapSlot != null)
            {
                return weapSlot;
            }
            var armorSlot = GetArmorSlot(slot);
            if (armorSlot != null)
            {
                return armorSlot;
            }
            return null;
        }


        private void Awake()
        {
            serverPlayer = playerEntity.player;
            serverPlayer.OnInitialize += Initialize;
            serverPlayer.OnDataSave += SaveSkillData;
            serverPlayer.OnDelete += UnRegisterEventHandlers;
        }


        private void SaveSkillData(ConnectedPlayer saveData)
        {
            if(saveData.equipData == null) 
            {
                saveData.equipData = new PlayerEquipData();
            }
            else 
            {
                saveData.equipData.equippedItems.Clear();
            }
            if (primaryHand.equippedItem != null)
            {
                saveData.equipData.equippedItems.Add(new EquipItem { slotID = (int)primaryHand.slot, itemID = primaryHand.equippedItem.itemData.id });
            }
            if (secondaryHand.equippedItem != null)
            {
                saveData.equipData.equippedItems.Add(new EquipItem { slotID = (int)secondaryHand.slot, itemID = secondaryHand.equippedItem.itemData.id });
            }
            if (chest.equippedItem != null)
            {
                saveData.equipData.equippedItems.Add(new EquipItem { slotID = (int)chest.slot, itemID = chest.equippedItem.itemData.id });
            }
            if (arms.equippedItem != null)
            {
                saveData.equipData.equippedItems.Add(new EquipItem { slotID = (int)arms.slot, itemID = arms.equippedItem.itemData.id });
            }
            if (legs.equippedItem != null)
            {
                saveData.equipData.equippedItems.Add(new EquipItem { slotID = (int)legs.slot, itemID = legs.equippedItem.itemData.id });
            }
            if (primaryHand.equippedSkill != null) 
            {
                saveData.equipData.primaryHandSkillID = primaryHand.equippedSkill.skillData.skillID;
            }
            else 
            {
                saveData.equipData.primaryHandSkillID = SkillData.NO_SKILL_INDEX;
            }
            if (secondaryHand.equippedSkill != null) 
            {
                saveData.equipData.secondaryHandSkillID = secondaryHand.equippedSkill.skillData.skillID;
            }
            else
            {
                saveData.equipData.secondaryHandSkillID = SkillData.NO_SKILL_INDEX;
            }
        }

        private void LoadSkillData(ConnectedPlayer saveData) 
        {
            if (saveData.equipData != null)
            {
                foreach (var item in saveData.equipData.equippedItems)
                {
                    GetItemSlot((EquipSlot)item.slotID).EquipItem(itemLookup.GetItem((uint)item.itemID));
                }
                primaryHand.EquipSkill(skillLookup.GetSkill((uint)saveData.equipData.primaryHandSkillID));
                secondaryHand.EquipSkill(skillLookup.GetSkill((uint)saveData.equipData.secondaryHandSkillID));
            }
        }


        private void Initialize(ConnectedPlayer saveData, IClient client)
        {
            LoadSkillData(saveData);
            playerEntity.OnEnteredRoom += AddListeners;
            playerEntity.OnLeftRoom += RemoveListeners;
        }

        private void AddListeners(Room obj)
        {
            serverPlayer.client.MessageReceived += HandleSkillMessage;
        }




        private void HandleSkillMessage(object sender, MessageReceivedEventArgs e)
        {
            switch ((ClientTags)e.Tag) 
            {
                case ClientTags.ChangeWeapon:
                    {
                        using(var message = e.GetMessage()) 
                        {
                            using (var reader = message.GetReader()) 
                            {
                                var data = reader.ReadSerializable<EquipRequest>();
                                GetItemSlot(data.equipSlot).EquipItem(itemLookup.GetItem(data.itemID));
                            }
                        }
                        break;
                    }
                case ClientTags.ChangeSkill:
                    {
                        using (var message = e.GetMessage())
                        {
                            using (var reader = message.GetReader())
                            {
                                var data = reader.ReadSerializable<SkillChangeRequest>();
                                GetWeaponSlot(data.slot).EquipSkill(skillLookup.GetSkill(data.skillInfo.skillID));
                            }
                        }
                        break;
                    }
            }
        }

        private void RemoveListeners(Room obj)
        {
            serverPlayer.client.MessageReceived -= HandleSkillMessage;
        }
        private void UnRegisterEventHandlers()
        {
            playerEntity.OnEnteredRoom -= AddListeners;
            playerEntity.OnLeftRoom -= RemoveListeners;
        }


    }
}