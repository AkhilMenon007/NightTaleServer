using DarkRift;
using DarkriftSerializationExtensions;
using UnityEngine;




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

public struct VRTransformData : IDarkRiftSerializable
{
    /// <summary>
    /// Transforms of VR Objects 0 for Headset, 1 for right controller and 2 for left controller
    /// </summary>
    public TransformData[] vrTransforms;
    public void Deserialize(DeserializeEvent e)
    {
        vrTransforms = new TransformData[3];
        e.Reader.ReadSerializablesInto(vrTransforms, 0);
    }
    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(vrTransforms);
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

