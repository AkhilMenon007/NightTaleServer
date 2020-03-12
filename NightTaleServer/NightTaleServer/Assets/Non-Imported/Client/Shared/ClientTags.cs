using System;

namespace FYP.Shared
{
    public enum ClientTags
    {
        LoginRequest,
        LogoutRequest,

        /// <summary>
        /// Request server to join room
        /// </summary>
        JoinRoomRequest,
        /// <summary>
        /// Send server message acknowledging the scene has been loaded
        /// </summary>
        RoomDataRequest,
        /// <summary>
        /// Request server to send the template and instance ID of the current room typically for first login
        /// </summary>
        JoinLastRoomRequest,
        /// <summary>
        /// Requests server to create a room with a given template ID
        /// </summary>
        CreateRoomRequest,

        /// <summary>
        /// This tag is used to chain to server tags
        /// </summary>
        Unused
    }
    //Tags for messages from Server to Client
    public enum ServerTags 
    {
        LoginRequestAccepted = ClientTags.Unused,
        LoginRequestDenied,
        /// <summary>
        /// Send message to client indicating a number of players have joined the room
        /// </summary>
        PlayerJoinedRoom,
        /// <summary>
        /// Send message to client giving him sceneID and instanceID of the room he joined
        /// </summary>
        JoinRoom,
        /// <summary>
        /// Send message to client giving him 
        /// </summary>
        RoomData,
        /// <summary>
        /// Send message to client to unload the present game scene
        /// </summary>
        LeftRoom,
        /// <summary>
        /// Send message to client indicating an entity has been created at the server
        /// </summary>
        EntitySpawned
    }

}