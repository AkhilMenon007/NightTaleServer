using DarkRift;

namespace FYP.Server
{
    public interface IServerWritable
    {
        void WriteUpdateDataToWriter(DarkRiftWriter writer);
        void WriteStateDataToWriter(DarkRiftWriter writer);
        void ResetUpdateData();
    }
}