﻿using DarkRift;
using DarkriftSerializationExtensions;
using UnityEngine;

public struct RoomCreationRequest : IDarkRiftSerializable
{
    public ushort templateID;

    public void Deserialize(DeserializeEvent e)
    {
        templateID = e.Reader.ReadUInt16();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(templateID);
    }
}

public struct RoomJoinRequest : IDarkRiftSerializable
{
    public uint roomInstanceID;

    public void Deserialize(DeserializeEvent e)
    {
        roomInstanceID = e.Reader.ReadUInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(roomInstanceID);
    }
}

public struct RoomData : IDarkRiftSerializable
{
    public ushort templateID;
    public uint roomInstanceID;
    public void Deserialize(DeserializeEvent e)
    {
        templateID = e.Reader.ReadUInt16();
        roomInstanceID = e.Reader.ReadUInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(templateID);
        e.Writer.Write(roomInstanceID);
    }
    //Following this will be a list of entitydata
}

/// <summary>
/// Base Data to write entityData
/// </summary>
public struct EntityData : IDarkRiftSerializable 
{
    /// <summary>
    /// The type of prefab to be instantiated
    /// </summary>
    public ushort entityType;
    /// <summary>
    /// The unique id corresponding to the entity
    /// </summary>
    public uint entityID;
    public void Deserialize(DeserializeEvent e)
    {
        entityType = e.Reader.ReadUInt16();
        entityID = e.Reader.ReadUInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(entityType);
        e.Writer.Write(entityID);
    }

}

public struct RoomLeftMessage : IDarkRiftSerializable
{
    public ushort clientID;
    public uint roomID;
    public void Deserialize(DeserializeEvent e)
    {
        clientID = e.Reader.ReadUInt16();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(clientID);
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
    public bool vrEnabled;
    public TransformData transformData;
    public void Deserialize(DeserializeEvent e)
    {
        clientID = e.Reader.ReadUInt16();
        charID = e.Reader.ReadString();
        vrEnabled = e.Reader.ReadBoolean();
        transformData = e.Reader.ReadSerializable<TransformData>();
    }
    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(clientID);
        e.Writer.Write(charID);
        e.Writer.Write(vrEnabled);
        e.Writer.Write(transformData);
    }
}
public struct PlayerTransformData : IDarkRiftSerializable
{
    public ushort clientID;
    public bool vrEnabled;
    /// <summary>
    /// Transforms of VR Objects 0 for Headset, 1 for right controller and 2 for left controller
    /// </summary>
    public TransformData[] vrTransforms;
    public Vector3 position;
    public Quaternion rotation;
    public void Deserialize(DeserializeEvent e)
    {
        clientID = e.Reader.ReadUInt16();
        vrEnabled = e.Reader.ReadBoolean();
        if (vrEnabled) 
        {
            vrTransforms = new TransformData[3];
            e.Reader.ReadSerializablesInto(vrTransforms, 0);
            position = e.Reader.ReadVector3();
        }
        else 
        {
            position = e.Reader.ReadVector3();
            rotation = e.Reader.ReadQuaternionCompressed();
        }
    }
    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(clientID);
        e.Writer.Write(vrEnabled);
        if (vrEnabled) 
        {
            e.Writer.Write(vrTransforms);
            e.Writer.WriteVector3(position);
        }
        else 
        {
            e.Writer.WriteVector3(position);
            e.Writer.WriteQuaternionCompressed(rotation);
        }
    }
}

public struct TransformData : IDarkRiftSerializable
{
    public Vector3 position;
    public Quaternion rotation;
    public void Deserialize(DeserializeEvent e)
    {
        position = e.Reader.ReadVector3();
        rotation = e.Reader.ReadQuaternionCompressed();
    }
    public void Serialize(SerializeEvent e)
    {
        e.Writer.WriteVector3(position);
        e.Writer.WriteQuaternionCompressed(rotation);
    }
}