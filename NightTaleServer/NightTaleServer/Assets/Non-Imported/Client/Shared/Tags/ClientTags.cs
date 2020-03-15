using System;

namespace FYP.Shared
{
    public enum ClientTags
    {
        LoginRequest,
        LogoutRequest,

        //---------ROOM TAGS--------------

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

        //----- UPDATE DATA------

        /// <summary>
        /// Update data sent to server every tick
        /// </summary>
        ClientUpdate,


        /// <summary>
        /// This tag is used to chain to server tags
        /// </summary>
        Unused
    }
}