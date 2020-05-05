using DarkRift;
using DarkriftSerializationExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Shared
{
    /// <summary>
    /// Request asking for item of itemID to be equipped on the equipSlot
    /// </summary>
    public struct EquipRequest : IDarkRiftSerializable
    {
        public EquipSlot equipSlot;
        public ushort itemID;
        public void Deserialize(DeserializeEvent e)
        {
            equipSlot = e.Reader.ReadSingleEquipSlot();
            itemID = e.Reader.ReadUInt16();
        }
        public void Serialize(SerializeEvent e)
        {
            e.Writer.WriteSingleEquipSlot(equipSlot);
            e.Writer.Write(itemID);
        }
    }

    public struct EquipMessage : IDarkRiftSerializable 
    {
        public EquipRequest slotAndID;
        /// <summary>
        /// This value is to be touched only if the slot is a weapon slot
        /// </summary>
        public SkillSetMessage activeSkill;

        public void Deserialize(DeserializeEvent e)
        {
            e.Reader.ReadSerializableInto(ref slotAndID);
            if(slotAndID.equipSlot == EquipSlot.PrimaryHand || slotAndID.equipSlot == EquipSlot.SecondaryHand) 
            {
                e.Reader.ReadSerializableInto(ref activeSkill);
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(slotAndID);
            if (slotAndID.equipSlot == EquipSlot.PrimaryHand || slotAndID.equipSlot == EquipSlot.SecondaryHand)
            {
                e.Writer.Write(activeSkill);
            }
        }
    }


    public struct SkillSetMessage : IDarkRiftSerializable
    {
        public ushort skillID;
        public void Deserialize(DeserializeEvent e)
        {
            skillID = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(skillID);
        }
    }
    /// <summary>
    /// Message Sent from server to client signifying a change in equipped weapon for a player 
    /// </summary>
    public struct EquipChangeMessage : IDarkRiftSerializable
    {
        public ushort clientID;
        public EquipMessage equipData;

        public void Deserialize(DeserializeEvent e)
        {
            clientID = e.Reader.ReadUInt16();
            e.Reader.ReadSerializableInto(ref equipData);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(clientID);
            e.Writer.Write(equipData);
        }
    }
    /// <summary>
    /// Message Requesting a skill to be changed on a weapon
    /// </summary>
    public struct SkillChangeRequest : IDarkRiftSerializable
    {
        public EquipSlot slot;
        public SkillSetMessage skillInfo;
        public void Deserialize(DeserializeEvent e)
        {
            slot = e.Reader.ReadSingleEquipSlot();
            e.Reader.ReadSerializableInto(ref skillInfo);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.WriteSingleEquipSlot(slot);
            e.Writer.Write(skillInfo);
        }
    }

    /// <summary>
    /// Message Sent from Server to clients indicating that a client has changed their skill on the given slot to the new value
    /// </summary>
    public struct SkillChangeMessage : IDarkRiftSerializable
    {
        public EquipSlot slot;
        public SkillSetMessage skillInfo;
        public void Deserialize(DeserializeEvent e)
        {
            slot = e.Reader.ReadSingleEquipSlot();
            e.Reader.ReadSerializableInto(ref skillInfo);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.WriteSingleEquipSlot(slot);
            e.Writer.Write(skillInfo);
        }
    }



}