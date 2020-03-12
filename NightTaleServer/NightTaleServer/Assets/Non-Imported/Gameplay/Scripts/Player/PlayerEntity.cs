﻿using DarkRift;
using DarkRift.Server;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    [RequireComponent(typeof(ServerPlayer))]
    public class PlayerEntity : ServerNetworkEntity
    {
        public ServerPlayer player { get; private set; }
        public bool vrEnabled { get; private set; }
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
            OnEnteredRoom += PlayerJoinedRoomCallback;
            OnLeftRoom += PlayerLeftRoomCallback;
            client.MessageReceived += RoomDataRequestCallback;
            client.MessageReceived += JoinLastRoomRequestCallback;
        }

        private void JoinLastRoomRequestCallback(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == (ushort)ClientTags.JoinLastRoomRequest) 
            {
                player.client.MessageReceived -= JoinLastRoomRequestCallback;
                JoinLastRoom(player.playerData);
            }
        }
        private void PlayerJoinedRoomCallback(Room obj)
        {
            //Send data about player to others
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(new EntityData() { entityID = entityID, entityType = entityType });
                WriteDataToWriter(writer);
                using (var message = Message.Create((ushort)ServerTags.EntitySpawned, writer))
                {
                    room.SendMessageToEntireRoomExceptPlayer(this, message, SendMode.Reliable);
                }
            }
            //Send room intialize message to player
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(new RoomJoinData() { templateID = obj.clientSceneIndex, instanceID = obj.instanceID });
                using (var message = Message.Create((ushort)ServerTags.JoinRoom, writer))
                {
                    player.client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
        private void PlayerLeftRoomCallback(Room obj)
        {
            using(var writer = DarkRiftWriter.Create()) 
            {
                writer.Write(new RoomLeftMessage() { clientID = player.client.ID , roomID = obj.instanceID});
                using(var message = Message.Create((ushort)ServerTags.LeftRoom, writer)) 
                {
                    room.SendMessageToEntireRoom(message, SendMode.Reliable);
                }
            }
        }
        private void RoomDataRequestCallback(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == (ushort)ClientTags.RoomDataRequest) 
            {
                using(var message = e.GetMessage()) 
                {
                    using (var writer = DarkRiftWriter.Create())
                    {
                        writer.Write(new RoomData() { roomInstanceID = room.instanceID,templateID = room.roomTemplate.templateID });
                        foreach (var entity in roomManager.GetRoom(room.instanceID).networkEntities)
                        {
                            writer.Write(new EntityData() { entityID = entity.entityID, entityType = entity.entityType });
                            entity.WriteDataToWriter(writer);
                        }
                        using (var reply = Message.Create((ushort)ServerTags.RoomData, writer))
                        {
                            player.client.SendMessage(reply, SendMode.Reliable);
                        }
                    }
                }
            }
        }
        private void JoinLastRoom(ConnectedPlayer data)
        {
            var oldID = data.positionalData.instanceID;
            var oldPos = data.positionalData.position;
            var joinedRoom = roomManager.EnterRoom(this, data.positionalData.templateID, data.positionalData.instanceID);
            if (joinedRoom == null)
            {
                Debug.LogError($"{player.client.ID} Failed to join any Room error!!");
                PlayerSessionManager.instance.LogoutClient(player.client);
                return;
            }
            if (oldID == joinedRoom.instanceID)
            {
                position = oldPos;
            }
        }
        private void DeletePlayer()
        {
            player.OnDataSave -= SavePlayerData;
            player.OnDelete -= DeletePlayer;
            OnEnteredRoom -= PlayerJoinedRoomCallback;
            OnLeftRoom -= PlayerLeftRoomCallback;
            player.client.MessageReceived -= RoomDataRequestCallback;
            player.client.MessageReceived -= JoinLastRoomRequestCallback;


            roomManager.LeaveRoom(this,room);
        }
        private void SavePlayerData(ConnectedPlayer obj)
        {
            obj.positionalData.instanceID = room.instanceID;
            obj.positionalData.templateID = room.roomTemplate.templateID;
            obj.positionalData.position = position;
        }
        public override void WriteDataToWriter(DarkRiftWriter writer)
        {
            writer.Write(new NewPlayerData() 
            {   
                charID = player.charID,
                clientID = player.client.ID,
                vrEnabled = vrEnabled,
                transformData = new TransformData() 
                {
                    position = position, 
                    rotation = rotation
                }
            });
        }
    }
}