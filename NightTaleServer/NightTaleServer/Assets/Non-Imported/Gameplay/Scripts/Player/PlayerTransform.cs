using DarkRift.Server;
using FYP.Server.RoomManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    [RequireComponent(typeof(ServerPlayer))]
    public class PlayerTransform : NetworkTransform
    {
        public ServerPlayer player { get; private set; }
        private RoomManager roomManager => RoomManager.instance;
        private void Awake()
        {
            player = GetComponent<ServerPlayer>();
            player.OnInitialize += SetInitialData;
        }
        private void SetInitialData(PlayerData data, IClient client)
        {
            var joinedRoom = roomManager.EnterRoom(player, data.positionalData.templateID, data.positionalData.instanceID);
            if(joinedRoom == null) 
            {
                Debug.LogError($"{client.ID} Failed to join any Room error!!");
                PlayerSessionManager.instance.LogoutClient(client);
            }
        }
    }
}