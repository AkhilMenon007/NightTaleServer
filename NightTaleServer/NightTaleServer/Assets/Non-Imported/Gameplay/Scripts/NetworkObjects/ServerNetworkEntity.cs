using DarkRift;
using DarkRift.Server;
using FYP.Server.Player;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using UnityEngine;

namespace FYP.Server
{
    public abstract class ServerNetworkEntity : MonoBehaviour
    {
        [SerializeField]
        private ushort _entityType = 0;
        [Tooltip("How often should the LOR be checked")]
        [SerializeField]
        private float lorCheckInterval = 5f;
        public bool serverOwned => owner==null;
        public ServerPlayer owner { get; private set; }
        public ushort entityType => _entityType;
        public uint entityID { get; set; }
        public Room room { get; set; }
        public LocalityOfRelevance lor { get; set; }
        [SerializeField]
        private EntityOutputWriter _unreliableOutputWriter = null;
        [SerializeField]
        private EntityOutputWriter _reliableOutputWriter = null;


        public EntityOutputWriter unreliableOutputWriter => _unreliableOutputWriter;
        public EntityOutputWriter reliableOutputWriter => _reliableOutputWriter;

        public Vector3 position { get => transform.position; set => SetObjectPosition(value); }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }

        public Action<Room> OnLeftRoom { get; set; }
        public Action<Room> OnEnteredRoom { get; set; }

        public Action<LocalityOfRelevance> OnLORChanged { get; set; }



        /// <summary>
        /// Initialization Data required by clients to reproduce the entity clientside
        /// </summary>
        public abstract void WriteNewEntityDataToWriter(DarkRiftWriter writer);

        private int lorCheckMaxValue = 0;
        private int lorCheckCounter = 0;

        protected virtual void Awake()
        {
            lorCheckMaxValue = Mathf.CeilToInt(lorCheckInterval / Time.fixedDeltaTime);
            lorCheckCounter = lorCheckMaxValue;

            OnEnteredRoom += SendRoomJoinedMessageToOthers;
            OnLeftRoom += SendRoomExitedMessageToOthers;
            OnLORChanged += SendStateDataToOthers;
        }

        private void SendStateDataToOthers(LocalityOfRelevance target)
        {
            using (var writer = DarkRiftWriter.Create())
            {
                foreach (var adjLor in target.adjacentLoRs)
                {
                    foreach (var item in adjLor.objects)
                    {
                        item.unreliableOutputWriter.WriteUpdateDataToWriter(writer);
                        item.reliableOutputWriter.WriteUpdateDataToWriter(writer);
                    }
                }
                if (writer.Length != 0)
                {
                    using (var message = Message.Create((ushort)ServerTags.StateData, writer))
                    {
                        foreach (var player in lor.GetPlayerClients())
                        {
                            player.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (lorCheckCounter <= 0) 
            {
                if (room != null) 
                {
                    if (!lor.bounds.Contains(position)) 
                    {
                        var target = room.GetLOR(position);
                        if (target != lor && target != null)
                        {
                            Debug.Log($"LOR Changed from ({lor.index[0]},{lor.index[2]}) to ({target.index[0]},{target.index[2]})");
                            lor.TransferObject(this, target);
                            OnLORChanged?.Invoke(target);
                        }
                    }
                }
                lorCheckCounter = lorCheckMaxValue;
            }
            else 
            {
                lorCheckCounter--;
            }
        }

        private void SendRoomExitedMessageToOthers(Room obj)
        {
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(new EntityDestroyData() { entityID = entityID });
                using (var message = Message.Create((ushort)ServerTags.EntityDestroyed, writer))
                {
                    if (!(this is PlayerEntity thisPlayer))
                    {
                        foreach (var roomPlayer in room.players)
                        {
                            roomPlayer.player.client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                    else 
                    {
                        foreach (var roomPlayer in room.players)
                        {
                            if (roomPlayer != thisPlayer) 
                            {
                                roomPlayer.player.client.SendMessage(message, SendMode.Reliable);
                            }
                        }
                    }
                }
            }
        }


        private void SendRoomJoinedMessageToOthers(Room room) 
        {
            using (var writer = DarkRiftWriter.Create())
            {
                if (serverOwned) 
                {
                    writer.Write(new EntityCreationData() { entityID = entityID, entityType = entityType, serverOwned = serverOwned });
                }
                else 
                {
                    writer.Write(new EntityCreationData() { entityID = entityID, entityType = entityType, serverOwned = serverOwned, ownerID = owner.client.ID });
                }

                WriteNewEntityDataToWriter(writer);
                using (var message = Message.Create((ushort)ServerTags.EntitySpawned, writer))
                {
                    if(this is PlayerEntity player) 
                    {
                        foreach (var allPlayer in room.players)
                        {
                            if (allPlayer != player) 
                            {
                                allPlayer.player.client.SendMessage(message, SendMode.Reliable);
                            }
                        }
                    }
                    else 
                    {
                        foreach (var allPlayer in room.players)
                        {
                            allPlayer.player.client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }

        }

        private void SetObjectPosition(Vector3 value)
        {
            transform.position = value;
        }
        public void SetOwner(ServerPlayer owner) 
        {
            this.owner = owner;
        }
        public void RemoveOwner() 
        {
            owner = null;
        }
        protected virtual void OnDestroy()
        {
            OnEnteredRoom -= SendRoomJoinedMessageToOthers;
            OnLeftRoom -= SendRoomExitedMessageToOthers;
        }
    }
}