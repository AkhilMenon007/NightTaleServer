using DarkRift;

namespace FYP.Server.Player
{
    public interface IServerReadable
    {
        void HandlePlayerInputFromReader(DarkRiftReader reader);
    }
}