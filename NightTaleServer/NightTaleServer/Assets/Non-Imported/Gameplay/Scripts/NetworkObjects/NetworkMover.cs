using DarkRift;
using FYP.Server.Player;
using FYP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(ServerNetworkEntity))]
    public class NetworkMover : MonoBehaviour
    {
        [SerializeField]
        private float initialSpeed = 5f;

        public float speed { get; set; }

        private ServerNetworkEntity networkEntity = null;
        private MovementInputHandler messageHandler = null;

        private void Awake()
        {
            speed = initialSpeed;
            networkEntity = GetComponent<ServerNetworkEntity>();
            networkEntity.OnOwnerAssigned += RegisterListeners;
            networkEntity.OnOwnerRemoved += RemoveListeners;
        }
        private void RegisterListeners()
        {
            messageHandler = networkEntity.owner.inputController.GetMessageHandler<MovementInputHandler>((ushort)ClientDataTags.MoveObject);
            messageHandler.RegisterMover(networkEntity.entityID, this);
        }

        public void ReadDataFromReader(DarkRiftReader reader) 
        {
            var data = reader.ReadSerializable<MovementData>();
            networkEntity.rotation = data.rotation;
            var maxDir = Mathf.Max(data.movementVector.x, data.movementVector.y, data.movementVector.z);
            if(maxDir > 1f) 
            {
                data.movementVector /= maxDir;
            }
            networkEntity.position += data.movementVector * speed * Time.fixedDeltaTime;
        }


        private void RemoveListeners(ServerPlayer obj)
        {
            messageHandler.UnregisterMover(networkEntity.entityID);
            messageHandler = null;
        }

    }
}