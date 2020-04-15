using DarkRift;
using DarkRift.Server;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    public class PlayerEntity : ServerNetworkEntity
    {
        [SerializeField]
        private VRPlayerHandler vrHandler = null;
        public ServerPlayer player { get; private set; }
        public VRChangeData vrData { get; private set; }
        private RoomManager roomManager => RoomManager.instance;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<ServerPlayer>();
            player.OnInitialize += SetInitialData;
        }

        private void SetInitialData(ConnectedPlayer data, IClient client)
        {
            SetOwner(player);
            player.OnDataSave += SavePlayerData;
            player.OnDelete += DeletePlayer;
            OnEnteredRoom += PlayerJoinedRoomCallback;
            OnLeftRoom += PlayerLeftRoomCallback;
            client.MessageReceived += RoomDataRequestCallback;
            client.MessageReceived += JoinLastRoomRequestCallback;
        }


        public override void WriteNewEntityDataToWriter(DarkRiftWriter writer)
        {
            writer.Write(new NewPlayerData()
            {
                charID = player.charID,
                clientID = player.client.ID,
                vrData = vrData,
                transformData = new TransformData()
                {
                    position = position,
                    rotation = rotation
                }
            });
        }
        private void JoinLastRoomRequestCallback(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == (ushort)ClientTags.ResumeGame) 
            {
                player.client.MessageReceived -= JoinLastRoomRequestCallback;

                using(var message = e.GetMessage()) 
                {
                    using(var reader = message.GetReader()) 
                    {
                        var data = reader.ReadSerializable<ResumeGameMessage>();
                        vrData = data.vrData;
                        vrHandler.enabled = vrData.state;
                    }
                }
                JoinLastRoom(player.playerData);
            }
        }
        private void PlayerJoinedRoomCallback(Room obj)
        {
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
                writer.Write(new RoomLeftMessage() { roomID = obj.instanceID});
                using(var message = Message.Create((ushort)ServerTags.LeftRoom, writer)) 
                {
                    player.client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
        private void RoomDataRequestCallback(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == (ushort)ClientTags.RoomDataRequest) 
            {
                using (var writer = DarkRiftWriter.Create())
                {
                    writer.Write(new RoomData() { roomInstanceID = room.instanceID,templateID = room.roomTemplate.templateID });
                    foreach (var entity in roomManager.GetRoom(room.instanceID).networkEntities.Values)
                    {
                        if (entity.serverOwned) 
                        {
                            writer.Write(new EntityCreationData() { entityID = entity.entityID, entityType = entity.entityType, serverOwned = true });
                        }
                        else 
                        {
                            writer.Write(new EntityCreationData() { entityID = entity.entityID, entityType = entity.entityType, serverOwned = false ,ownerID = entity.owner.client.ID});
                        }
                        entity.WriteNewEntityDataToWriter(writer);
                        entity.outputWriter.WriteStateDataToWriter(writer);
                    }
                    using (var reply = Message.Create((ushort)ServerTags.RoomData, writer))
                    {
                        player.client.SendMessage(reply, SendMode.Reliable);
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
            roomManager.LeaveRoom(this, room);
            RemoveOwner();

            player.OnDataSave -= SavePlayerData;
            player.OnDelete -= DeletePlayer;
            OnEnteredRoom -= PlayerJoinedRoomCallback;
            OnLeftRoom -= PlayerLeftRoomCallback;
            player.client.MessageReceived -= RoomDataRequestCallback;
            player.client.MessageReceived -= JoinLastRoomRequestCallback;
        }
        private void SavePlayerData(ConnectedPlayer obj)
        {
            obj.positionalData.instanceID = room.instanceID;
            obj.positionalData.templateID = room.roomTemplate.templateID;
            obj.positionalData.position = position;
        }

    }
}