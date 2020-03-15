using DarkRift;
using FYP.Shared;

namespace FYP.Server.Player
{
    public interface IServerReadable
    {
        void HandlePlayerInputFromReader(DarkRiftReader reader, ClientDataTags tag);
    }
}