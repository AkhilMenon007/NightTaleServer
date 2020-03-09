using DarkRift.Server;
using FYP.Server.RoomManagement;
using System;
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


        private void SetInitialData(ConnectedPlayer data, IClient client)
        {
            player.OnDataSave += SavePlayerData;
            player.OnDelete += DeletePlayer;
            JoinLastRoom(data);
        }

        private void JoinLastRoom(ConnectedPlayer data)
        {
            var oldID = data.positionalData.instanceID;
            var oldPos = data.positionalData.position;
            var joinedRoom = roomManager.EnterRoom(player, data.positionalData.templateID, data.positionalData.instanceID);
            if (joinedRoom == null)
            {
                Debug.LogError($"{player.client.ID} Failed to join any Room error!!");
                PlayerSessionManager.instance.LogoutClient(player.client);
            }
            if (oldID == joinedRoom.roomID)
            {
                position = data.positionalData.position;
            }
        }

        private void DeletePlayer()
        {
            roomManager.LeaveRoom(player);
        }
        private void SavePlayerData(ConnectedPlayer obj)
        {
            obj.positionalData.instanceID = room.roomID;
            obj.positionalData.templateID = room.roomTemplate.templateID;
            obj.positionalData.position = position;
        }
    }
}