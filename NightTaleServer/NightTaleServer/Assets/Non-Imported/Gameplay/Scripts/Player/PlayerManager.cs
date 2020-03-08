using UnityEngine;
using FYP.Server.RoomManagement;
using DarkRift.Server;
using System;
using System.Collections.Generic;

namespace FYP.Server.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        private ServerPlayer playerPrefab = null;

        public static PlayerManager instance = null;
        private PlayerSessionManager sessionManager => PlayerSessionManager.instance;
        private RoomManager roomManager => RoomManager.instance;

        private readonly Dictionary<IClient, ServerPlayer> playerObjectLookUp = new Dictionary<IClient, ServerPlayer>();
        private void Awake()
        {
            if(instance == null) 
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else 
            {
                Destroy(this);
                return;
            }
        }

        private void OnEnable()
        {
            sessionManager.OnClientLoggedIn += ClientLoggedIn;
            sessionManager.OnClientLoggedOut += ClientLoggedOut;
        }

        private void ClientLoggedIn(IClient obj)
        {
            var data = sessionManager.GetClientData(obj);
            if(data == null) 
            {
                var charID = sessionManager.GetCharID(obj);
                if (string.IsNullOrEmpty(charID)) 
                {
                    sessionManager.LogoutClient(obj);
                    return;
                }
                data = GetNewPlayerData(charID);
                sessionManager.SetClientData(obj,data);
            }
            var playerObj = Instantiate(playerPrefab);
            playerObjectLookUp[obj] = playerObj;
            playerObj.Initialize(data,obj);
        }

        private PlayerData GetNewPlayerData(string charID) 
        {
            var res = new PlayerData
            {
                charID = charID,
                positionalData = new PlayerPositionalData 
                { 
                    instanceID = roomManager.defaultRoomID, 
                    position = roomManager.defaultRoomTemplate.centre, 
                    templateID = roomManager.defaultRoomTemplate.templateID 
                }
            };
            return res;
        }

        private void ClientLoggedOut(IClient obj)
        {

        }

        private void OnDisable()
        {
            sessionManager.OnClientLoggedIn -= ClientLoggedIn;
            sessionManager.OnClientLoggedOut -= ClientLoggedOut;
        }
    }
}