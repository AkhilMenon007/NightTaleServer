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

        public ServerPlayer GetPlayer(IClient client) 
        {
            return playerObjectLookUp.ContainsKey(client) ? playerObjectLookUp[client] : null;
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

        private ConnectedPlayer GetNewPlayerData(string charID) 
        {
            var res = new ConnectedPlayer();
            res.charID = charID;
            res.positionalData = new PlayerPositionalData();
            var pos = res.positionalData;
            pos.instanceID = roomManager.defaultRoomID;
            pos.position = roomManager.defaultRoom.origin.position;
            pos.templateID = roomManager.defaultRoomTemplate.templateID;

            return res;
        }

        private void ClientLoggedOut(IClient obj)
        {
            if(playerObjectLookUp.TryGetValue(obj,out var res)) 
            {
                res.Delete();
                playerObjectLookUp.Remove(obj);
            }
        }

        private void OnDisable()
        {
            sessionManager.OnClientLoggedIn -= ClientLoggedIn;
            sessionManager.OnClientLoggedOut -= ClientLoggedOut;
        }
    }
}