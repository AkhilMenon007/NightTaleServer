using DarkRift.Server;
using FYP.Server.RoomManagement;
using System;
using UnityEngine;

namespace FYP.Server
{
    public class NetworkTransform : MonoBehaviour
    {
        public Room room { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }

        private RoomManager roomManager => RoomManager.instance;

        public LocalityOfRelevance lor { get; set; }

        public Action<LocalityOfRelevance> OnLOREntered { get; set; }
        public Action<LocalityOfRelevance> OnLORExited { get; set; }

    }
}