using DarkRift;
using FYP.Server.Player;
using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(ServerNetworkEntity))]
    public class ClientControlledMover : MonoBehaviour
    {

        [SerializeField]
        private float initialSpeed = 5f;

        [SerializeField]
        private float snapMaxMagnitude = 1f;

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

        public void ReadTranslationDataFromReader(DarkRiftReader reader, ClientDataTags tag)
        {
            if(tag == ClientDataTags.MoveObject) 
            {
                var data = reader.ReadSerializable<MovementData>();
                networkEntity.rotation = data.rotation;
                var maxDir = Mathf.Max(data.movementVector.x, data.movementVector.y, data.movementVector.z);
                if (maxDir > 1f)
                {
                    data.movementVector /= maxDir;
                }
                networkEntity.position += data.movementVector * speed * Time.fixedDeltaTime;
            }
            else if(tag == ClientDataTags.SetObjectPosition) 
            {
                var data = reader.ReadSerializable<MovementData>();
                networkEntity.rotation = data.rotation;
                var movVector = (data.movementVector - networkEntity.position);
                if (movVector.magnitude > snapMaxMagnitude) 
                {
                    movVector *= snapMaxMagnitude/movVector.magnitude;
                }
                networkEntity.position = networkEntity.position + movVector;
            }
        }


        private void RemoveListeners(ServerPlayer obj)
        {
            messageHandler.UnregisterMover(networkEntity.entityID);
            messageHandler = null;
        }
    }
}