using DarkRift;
using DarkriftSerializationExtensions;
using UnityEngine;

public struct MovementMessage : IDarkRiftSerializable
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
