using DarkRift;

namespace FYP.Shared.Login
{
    public struct LoginRequestData : IDarkRiftSerializable
    {
        public string CharID;
        public ushort Token;
        public LoginRequestData(string charID, ushort token)
        {
            CharID = charID;
            Token = token;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Token = e.Reader.ReadUInt16();
            CharID = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Token);
            e.Writer.Write(CharID);
        }
    }
}