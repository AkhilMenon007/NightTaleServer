using DarkRift;
using DarkRift.Server;
using FYP.Server.Player;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(EntityOutputWriter))]
    public abstract class ServerNetworkEntity : MonoBehaviour
    {
        [SerializeField]
        private ushort _entityType = 0;
        public bool serverOwned => owner==null;
        public ServerPlayer owner { get; private set; }
        public ushort entityType => _entityType;
        public uint entityID { get; set; }
        public Room room { get; set; }
        public LocalityOfRelevance lor { get; set; }

        public EntityOutputWriter outputWriter { get; private set; }

        public Vector3 position { get => transform.position; set => SetObjectPosition(value); }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }



        public Action<Room> OnLeftRoom { get; set; }
        public Action<Room> OnEnteredRoom { get; set; }

        public Action OnOwnerAssigned { get; set; }
        public Action<ServerPlayer> OnOwnerRemoved { get; set; }

        /// <summary>
        /// Initialization Data required by clients to reproduce the entity clientside
        /// </summary>
        public abstract void WriteNewEntityDataToWriter(DarkRiftWriter writer);

        protected virtual void Awake()
        {
            outputWriter = GetComponent<EntityOutputWriter>();
            OnEnteredRoom += SendRoomJoinedMessageToOthers;
            OnLeftRoom += SendRoomExitedMessageToOthers;
        }

        private void SendRoomExitedMessageToOthers(Room obj)
        {
            if(!(this is PlayerEntity)) 
            {
                using (var writer = DarkRiftWriter.Create())
                {
                    writer.Write(new EntityDestroyData() { entityID = entityID });
                    using (var message = Message.Create((ushort)ServerTags.EntityDestroyed, writer))
                    {
                        room.SendMessageToEntireRoom(message, SendMode.Reliable);
                    }
                }
            }
        }

        private void SendRoomJoinedMessageToOthers(Room room) 
        {
            using (var writer = DarkRiftWriter.Create())
            {
                writer.Write(new EntityCreationData() { entityID = entityID, entityType = entityType });
                WriteNewEntityDataToWriter(writer);
                using (var message = Message.Create((ushort)ServerTags.EntitySpawned, writer))
                {
                    if(this is PlayerEntity player) 
                    {
                        room.SendMessageToEntireRoomExceptPlayer(player, message, SendMode.Reliable);
                    }
                    else 
                    {
                        room.SendMessageToEntireRoom(message, SendMode.Reliable);
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
            if (owner != null) 
            {
                OnOwnerAssigned?.Invoke();
            }
        }
        public void RemoveOwner() 
        {
            if (owner != null) 
            {
                OnOwnerRemoved?.Invoke(owner);
            }
            owner = null;
        }
        protected virtual void OnDestroy()
        {
            OnEnteredRoom -= SendRoomJoinedMessageToOthers;
            OnLeftRoom -= SendRoomExitedMessageToOthers;
        }
    }
}