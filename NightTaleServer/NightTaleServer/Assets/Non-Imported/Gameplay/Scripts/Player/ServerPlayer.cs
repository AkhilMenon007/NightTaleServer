using DarkRift.Server;
using FYP.Server.RoomManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    [RequireComponent(typeof(PlayerEntity))]
    public class ServerPlayer : MonoBehaviour
    {
        public ConnectedPlayer playerData { get; private set; }
        public IClient client { get; private set; }
        public string charID { get; private set; }
        public PlayerEntity playerTransform { get; private set; } = null;
        public Action<ConnectedPlayer, IClient> OnInitialize { get; set; }
        public Action<ConnectedPlayer> OnDataSave { get; set; }
        public Action OnDelete{ get; set; }


        private void Awake()
        {
            playerTransform = GetComponent<PlayerEntity>();
        }
        public void Initialize(ConnectedPlayer playerData,IClient client) 
        {
            this.playerData = playerData;
            this.client = client;
            charID = PlayerSessionManager.instance.GetCharID(client);

            OnInitialize?.Invoke(playerData, client);
        }

        public void Save() 
        {
            OnDataSave?.Invoke(playerData);
        }
        public void Delete() 
        {
            OnDelete?.Invoke();
            Save();
            Destroy(gameObject);
        }
    }
}