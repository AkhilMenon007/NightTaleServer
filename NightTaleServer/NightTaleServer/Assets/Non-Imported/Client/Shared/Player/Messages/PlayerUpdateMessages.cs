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

public struct VRTransformData : IDarkRiftSerializable
{
    /// <summary>
    /// Transforms of VR Objects 0 for Headset, 1 for left controller and 2 for right controller
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

    public VRTransformData(Transform head, Transform left, Transform right)
    {
        vrTransforms = new TransformData[3];

        vrTransforms[0].position = head.transform.localPosition;
        vrTransforms[1].position = left.transform.localPosition;
        vrTransforms[2].position = right.transform.localPosition;

        vrTransforms[0].rotation = head.transform.localRotation;
        vrTransforms[1].rotation = left.transform.localRotation;
        vrTransforms[2].rotation = right.transform.localRotation;
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
