using FYP.Server.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Shared;
using DarkRift;
using FYP.Server.RoomManagement;

namespace FYP.Server
{
    [RequireComponent(typeof(ServerNetworkEntity))]
    public class PlayerControllableEntity : MonoBehaviour
    {
        public ServerNetworkEntity networkEntity { get; private set; } = null;
        public ushort ownerID => networkEntity.owner.client.ID;
        private Dictionary<ClientDataTags, IServerReadable> readables = new Dictionary<ClientDataTags, IServerReadable>();
        private void Awake()
        {
            networkEntity = GetComponent<ServerNetworkEntity>();
            networkEntity.OnEnteredRoom += AddListeners;
            networkEntity.OnLeftRoom += RemoveListeners;
        }

        private void AddListeners(Room room)
        {
            networkEntity.owner.inputController.RegisterControllableEntity(this);
        }

        public void RegisterReadable(IServerReadable readable,ClientDataTags tag) 
        {
            if (readable != null) 
            {
                readables[tag] = readable;
            }
        }
        public void UnregisterReadable(ClientDataTags tag) 
        {
            readables.Remove(tag);
        }

        public void ReadUpdateData(DarkRiftReader reader, ushort count)
        {
            while(reader.Position < count) 
            {
                var tag = (ClientDataTags)reader.ReadSerializable<ClientTag>().tag;
                if (readables.TryGetValue(tag, out var readable))
                {
                    readable.HandlePlayerInputFromReader(reader, tag);
                }
                else
                {
                    reader.Position = count;
                }
            }
        }

        private void RemoveListeners(Room room)
        {
            networkEntity.owner.inputController.UnregisterControllableEntity(this);
        }

    }
}