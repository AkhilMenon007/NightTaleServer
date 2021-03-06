﻿using DarkRift;
using DarkriftSerializationExtensions;
using System.Collections.Generic;
using UnityEngine;


namespace FYP.Shared
{
    public struct ResumeGameMessage : IDarkRiftSerializable
    {
        public VRChangeData vrData;
        public void Deserialize(DeserializeEvent e)
        {
            e.Reader.ReadSerializableInto(ref vrData);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(vrData);
        }
    }

    /// <summary>
    /// Message Sent from Client to Server
    /// </summary>
    public struct VRChangeData : IDarkRiftSerializable
    {
        public bool state;
        public float heightMLP;
        public float armMLP;
        public void Deserialize(DeserializeEvent e)
        {
            state = e.Reader.ReadBoolean();
            if (state)
            {
                heightMLP = e.Reader.ReadSingle();
                armMLP = e.Reader.ReadSingle();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is VRChangeData data &&
                   state == data.state &&
                   heightMLP == data.heightMLP &&
                   armMLP == data.armMLP;
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(state);
            if (state)
            {
                e.Writer.Write(heightMLP);
                e.Writer.Write(armMLP);
            }
        }

        public static bool operator ==(VRChangeData left, VRChangeData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VRChangeData left, VRChangeData right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return armMLP.GetHashCode() + heightMLP.GetHashCode() + state.GetHashCode();
        }
    }

    public struct VRChangeReply : IDarkRiftSerializable
    {
        public ushort clientID;
        public VRChangeData vrData;
        public void Deserialize(DeserializeEvent e)
        {
            clientID = e.Reader.ReadUInt16();
            e.Reader.ReadSerializableInto(ref vrData);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(clientID);
            e.Writer.Write(vrData);
        }
    }


    /// <summary>
    /// Base info regarding entity data
    /// </summary>
    public struct EntityCreationData : IDarkRiftSerializable
    {
        /// <summary>
        /// The type of prefab to be instantiated
        /// </summary>
        public ushort entityType;
        /// <summary>
        /// The unique id corresponding to the entity
        /// </summary>
        public uint entityID;
        /// <summary>
        /// Is the entity serverOwned
        /// </summary>
        public bool serverOwned;
        /// <summary>
        /// The client id of the owner of the entity
        /// </summary>
        public ushort ownerID;
        public void Deserialize(DeserializeEvent e)
        {
            entityType = e.Reader.ReadUInt16();
            entityID = e.Reader.ReadUInt32();
            serverOwned = e.Reader.ReadBoolean();
            if (!serverOwned)
            {
                ownerID = e.Reader.ReadUInt16();
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(entityType);
            e.Writer.Write(entityID);
            e.Writer.Write(serverOwned);
            if (!serverOwned)
            {
                e.Writer.Write(ownerID);
            }
        }
    }

    public struct EntityDestroyData : IDarkRiftSerializable
    {
        public uint entityID;

        public void Deserialize(DeserializeEvent e)
        {
            entityID = e.Reader.ReadUInt32();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(entityID);
        }
    }


    /// <summary>
    /// Sent From Server to client indicating the scene to load and the server instanceID associated with it
    /// </summary>
    public struct RoomJoinData : IDarkRiftSerializable
    {
        public ushort templateID;
        public uint instanceID;
        public void Deserialize(DeserializeEvent e)
        {
            templateID = e.Reader.ReadUInt16();
            instanceID = e.Reader.ReadUInt32();
        }
        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(templateID);
            e.Writer.Write(instanceID);
        }
    }


    /// <summary>
    /// Data corresponding to a player to be sent to clients when someone new connects
    /// </summary>
    public struct NewPlayerData : IDarkRiftSerializable
    {
        public ushort clientID;
        public string charID;
        public VRChangeData vrData;
        public TransformData transformData;
        public List<EquipMessage> equipData;
        public void Deserialize(DeserializeEvent e)
        {
            clientID = e.Reader.ReadUInt16();
            charID = e.Reader.ReadString();
            e.Reader.ReadSerializableInto(ref vrData);
            e.Reader.ReadSerializableInto(ref transformData);
            int equipCount = e.Reader.ReadByte();
            equipData = new List<EquipMessage>(equipCount);
            for (int i = 0; i < equipCount; i++)
            {
                equipData.Add(e.Reader.ReadSerializable<EquipMessage>());
            }
        }
        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(clientID);
            e.Writer.Write(charID);
            e.Writer.Write(vrData);
            e.Writer.Write(transformData);
            e.Writer.Write((byte)equipData.Count);
            foreach (var item in equipData)
            {
                e.Writer.Write(item);
            }
        }
    }
}