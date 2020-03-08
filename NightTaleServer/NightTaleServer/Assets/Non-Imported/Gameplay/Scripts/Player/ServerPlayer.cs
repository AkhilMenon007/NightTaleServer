using DarkRift.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.Player
{
    [RequireComponent(typeof(PlayerTransform))]
    public class ServerPlayer : MonoBehaviour
    {
        private PlayerData playerData;
        public IClient client { get; private set; }
        public string charID { get; private set; }
        public PlayerTransform playerTransform { get; private set; } = null;

        public Action<PlayerData, IClient> OnInitialize;
        public Action<PlayerData> OnDataSave;
        private void Awake()
        {
            playerTransform = GetComponent<PlayerTransform>();
        }
        public void Initialize(PlayerData playerData,IClient client) 
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
    }
}