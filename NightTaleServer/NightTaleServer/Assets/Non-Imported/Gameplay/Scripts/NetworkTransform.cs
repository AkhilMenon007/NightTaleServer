using DarkRift.Server;
using FYP.Server.RoomManagement;
using System;
using UnityEngine;

namespace FYP.Server
{
    public class NetworkTransform : MonoBehaviour
    {
        public Room room { get; set; }
        public LocalityOfRelevance lor { get; set; }
        public Vector3 position { get => transform.position; set => SetObjectPosition(value); }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }
        public Action<Room> OnLeftRoom { get; set; }
        public Action<Room> OnEnteredRoom { get; set; }

        private void SetObjectPosition(Vector3 value)
        {
            transform.position = value;
        }

    }
}