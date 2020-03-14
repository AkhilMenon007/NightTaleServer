using DarkRift;

namespace FYP.Server
{
    public interface IServerWritable
    {
        void WriteUpdateDataToWriter(DarkRiftWriter reader);
    }
}