using DarkRift;

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

public struct RoomLeftMessage : IDarkRiftSerializable
{
    public uint roomID;
    public void Deserialize(DeserializeEvent e)
    {
        roomID = e.Reader.ReadUInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(roomID);
    }
}
