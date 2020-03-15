using DarkRift;

namespace FYP.Server
{
    public interface IServerWritable
    {
        void WriteUpdateDataToWriter(DarkRiftWriter reader);
        void WriteStateDataToWriter(DarkRiftWriter writer);
        void ResetUpdateData();
    }
}