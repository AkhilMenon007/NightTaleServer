using DarkRift;
using FYP.Shared;

public struct ServerUpdateData : IDarkRiftSerializable
{
    /// <summary>
    /// ID corresponding to the object which is to be updated, every message here is required to identify its message tag
    /// </summary>
    public uint entityID;
    /// <summary>
    /// The amount of updates for the given Entity
    /// </summary>
    public ushort dataCount;
    public void Deserialize(DeserializeEvent e)
    {
        entityID = e.Reader.ReadUInt32();
        dataCount = e.Reader.ReadUInt16();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(entityID);
        e.Writer.Write(dataCount);
    }
}
public struct ServerUpdateTag : IDarkRiftSerializable
{
    public ushort tag;
    public ServerUpdateTag(ServerUpdateTags serverUpdateTag)
    {
        tag = (ushort)serverUpdateTag;
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
