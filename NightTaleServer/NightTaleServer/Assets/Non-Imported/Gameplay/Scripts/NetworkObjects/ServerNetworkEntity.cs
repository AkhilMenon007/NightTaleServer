using DarkRift;
using DarkRift.Server;
using FYP.Server.Player;
using FYP.Server.RoomManagement;
using System;
using UnityEngine;

namespace FYP.Server
{
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
        public Vector3 position { get => transform.position; set => SetObjectPosition(value); }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }
        public Action<Room> OnLeftRoom { get; set; }
        public Action<Room> OnEnteredRoom { get; set; }

        public Action OnOwnerAssigned { get; set; }
        public Action<ServerPlayer> OnOwnerRemoved { get; set; }

        /// <summary>
        /// Additional entity data to be written about this entity when synching entity states
        /// </summary>
        public abstract void WriteDataToWriter(DarkRiftWriter writer);

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
    }
}