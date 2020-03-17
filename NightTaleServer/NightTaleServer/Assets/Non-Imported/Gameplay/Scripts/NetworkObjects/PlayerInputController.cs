using DarkRift.Server;
using FYP.Server.Player;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(ServerPlayer))]
    public class PlayerInputController : MonoBehaviour
    {
        public ServerPlayer player { get; private set; }

        private readonly Dictionary<uint, PlayerControllableEntity> controlledEntities = new Dictionary<uint, PlayerControllableEntity>();
        private void Awake()
        {
            player = GetComponent<ServerPlayer>();
            player.OnInitialize += RegisterListeners;
            player.OnDelete += RemoveListeners;
        }


        private void RegisterListeners(ConnectedPlayer arg1, IClient arg2)
        {
            player.playerEntity.OnEnteredRoom += RoomEnteredCallback;
            player.playerEntity.OnLeftRoom += RoomExitedCallback;
        }

        private void RoomEnteredCallback(Room room)
        {
            player.client.MessageReceived += HandleClientInput;
        }



        public void RegisterControllableEntity(PlayerControllableEntity entity) 
        {
            if (entity != null) 
            {
                controlledEntities[entity.networkEntity.entityID] = entity;
            }
        }
        public void UnregisterControllableEntity(PlayerControllableEntity entity) 
        {
            if (entity != null) 
            {
                controlledEntities.Remove(entity.networkEntity.entityID);
            }
        }
        private void HandleClientInput(object sender, MessageReceivedEventArgs e)
        {
            if (e.Tag == (ushort)ClientTags.ClientUpdate) 
            {
                using(var message = e.GetMessage()) 
                {
                    using(var reader = message.GetReader()) 
                    {
                        while (reader.Position < reader.Length) 
                        {
                            var updateData = reader.ReadSerializable<ClientUpdateData>();
                            if(updateData.count < reader.Position) 
                            {
                                break;
                            }
                            if(controlledEntities.TryGetValue(updateData.entID,out var entity)) 
                            {
                                if(entity.ownerID == player.client.ID) 
                                {
                                    entity.ReadUpdateData(reader, updateData.count);
                                }
                            }
                            else 
                            {
                                reader.Position = updateData.count;
                            }
                        }
                    }
                }
            }
        }


        private void RoomExitedCallback(Room obj)
        {
            player.client.MessageReceived -= HandleClientInput;
        }
        private void RemoveListeners()
        {
            player.playerEntity.OnEnteredRoom -= RoomEnteredCallback;
            player.playerEntity.OnLeftRoom -= RoomExitedCallback;
        }
    }
}