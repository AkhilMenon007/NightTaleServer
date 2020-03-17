using DarkRift;
using FYP.Server.Player;
using FYP.Server.RoomManagement;
using FYP.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server
{
    [RequireComponent(typeof(PlayerControllableEntity))]
    public class ClientControlledMover : MonoBehaviour, IServerReadable
    {

        [SerializeField]
        private float initialSpeed = 5f;

        [SerializeField]
        private float snapMaxMagnitude = 1f;

        public float speed { get; set; }

        private ServerNetworkEntity networkEntity = null;
        private PlayerControllableEntity controller = null;
        private void Awake()
        {
            speed = initialSpeed;
            networkEntity = GetComponent<ServerNetworkEntity>();
            controller = GetComponent<PlayerControllableEntity>();


            networkEntity.OnEnteredRoom += RegisterListeners;
            networkEntity.OnLeftRoom += RemoveListeners;
        }
        private void RegisterListeners(Room room)
        {
            controller.RegisterReadable(this, ClientDataTags.MoveObject);
            controller.RegisterReadable(this, ClientDataTags.SetObjectPosition);
        }
        public void HandlePlayerInputFromReader(DarkRiftReader reader, ClientDataTags tag)
        {
            if (tag == ClientDataTags.MoveObject)
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
            else if (tag == ClientDataTags.SetObjectPosition)
            {
                var data = reader.ReadSerializable<MovementData>();
                networkEntity.rotation = data.rotation;
                var movVector = (data.movementVector - networkEntity.position);
                if (movVector.magnitude < snapMaxMagnitude)
                {
                    networkEntity.position = data.movementVector;
                }
            }
        }
        private void RemoveListeners(Room room)
        {
            controller.UnregisterReadable(ClientDataTags.MoveObject);
            controller.UnregisterReadable(ClientDataTags.SetObjectPosition);
        }
    }
}