namespace FYP.Shared
{
    //Tags for messages from Server to Client
    public enum ServerTags 
    {
        LoginRequestAccepted = ClientTags.Unused,
        LoginRequestDenied,


        //-------ROOM MANAGEMENT----

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
        EntitySpawned,
        /// <summary>
        /// Send message to client indicating an entity was destroyed at server
        /// </summary>
        EntityDestroyed,
        /// <summary>
        /// Send message to client with update data of all the entities in adjacent LORs
        /// </summary>
        UpdateData,
        /// <summary>
        /// Send message to client with state data of an entity which just changed LOR
        /// </summary>
        StateData,
        /// <summary>
        /// Similar to updatedata but uses TCP instead
        /// </summary>
        ReliableData,
        /// <summary>
        /// Send message to client signalling a client has changed in one of their equipped items
        /// </summary>
        SkillChange,
        /// <summary>
        /// Send message to client signalling a client changed one of their items
        /// </summary>
        ItemChange
    }

}