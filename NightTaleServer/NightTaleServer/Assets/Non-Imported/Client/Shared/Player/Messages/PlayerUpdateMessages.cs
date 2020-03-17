using DarkRift;
using DarkriftSerializationExtensions;
using FYP.Shared;
using UnityEngine;


public struct ClientTag : IDarkRiftSerializable 
{
    public ushort tag;

    public ClientTag(ClientDataTags tag)
    {
        this.tag = (ushort)tag;
    }
    public void Deserialize(DeserializeEvent e)
    {
        tag = e.Reader.ReadUInt16();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(tag);
    }
}
/// <summary>
/// Client Update Data sent every tick
/// </summary>
[System.Serializable]
public struct ClientUpdateData : IDarkRiftSerializable
{
    /// <summary>
    /// Tag used to identify the kind of update, every message here is required to identify its entity
    /// </summary>
    public uint entID;
    public ushort count;
    public void Deserialize(DeserializeEvent e)
    {
        entID = e.Reader.ReadUInt32();
        count = e.Reader.ReadUInt16();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(entID);
        e.Writer.Write(count);
    }
}
public struct EntityID : IDarkRiftSerializable
{
    public uint entID;
    public void Deserialize(DeserializeEvent e)
    {
        entID = e.Reader.ReadUInt32();
    }
    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(entID);
    }
}

/// <summary>
/// Data sent from client to server depicting the movement vector and the rotation of an object
/// </summary>
public struct MovementData : IDarkRiftSerializable
{
    public Vector3 movementVector;
    public Quaternion rotation;
    public void Deserialize(DeserializeEvent e)
    {
        movementVector = e.Reader.ReadVector3();
        rotation = e.Reader.ReadQuaternionCompressed();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.WriteVector3(movementVector);
        e.Writer.WriteQuaternionCompressed(rotation);
    }
}
