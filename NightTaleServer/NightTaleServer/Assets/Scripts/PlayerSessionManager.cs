using DarkRift;
using DarkRift.Server;
using FYP.Shared;
using FYP.Shared.Login;
using MasterServer.Darkrift.Shared.Models;
using MasterServer.DarkRift.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSessionManager : MonoBehaviour
{

    public static PlayerSessionManager instance = null;
    /// <summary>
    /// One time use tokens used for authenticating clients when they log into the server
    /// </summary>
    private Dictionary<string, ushort> sessionTokens = null;
    /// <summary>
    /// Dictionary containing data received from user server for corresponding charIDs
    /// </summary>
    private Dictionary<string, ServerCharData> charData = null;
    /// <summary>
    /// Mapping of the Clients which are connected to their corresponding characterIDs
    /// </summary>
    private Dictionary<IClient,string> loggedInCharacters = null;
    /// <summary>
    /// Mapping of various character IDs to their corresponding clients
    /// </summary>
    private Dictionary<string,IClient> loggedInCharactersByID = null;


    private IClientManager clientManager => ServerManager.Instance.Server.ClientManager;
    private MasterServerManager masterServerManager =>MasterServerManager.instance;
    private IClient masterServer => masterServerManager.masterServer;

    /// <summary>
    /// Callback for when a client successfully authenticates itself
    /// </summary>
    public Action<IClient> OnClientLoggedIn = null;
    /// <summary>
    /// Callback for when a client is logged out either naturally or forcefully
    /// </summary>
    public Action<IClient> OnClientLoggedOut = null;

    #region Initializations

    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(this);
            return;
        }
        sessionTokens = new Dictionary<string, ushort>();
        charData = new Dictionary<string, ServerCharData>();
        loggedInCharacters = new Dictionary<IClient,string>();
        loggedInCharactersByID = new Dictionary<string, IClient>();
    }
    private void OnEnable()
    {
        masterServerManager.OnMasterConnected += OnMasterConnected;
        masterServerManager.OnMasterDisconnected += OnMasterDisconnected;
        clientManager.ClientConnected += RegisterClient;
        clientManager.ClientDisconnected += UnRegisterClient;
    }
    private void RegisterClient(object sender, ClientConnectedEventArgs e)
    {
        e.Client.MessageReceived += ClientAuthRequest;
    }
    private void UnRegisterClient(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= ClientAuthRequest;
        if (loggedInCharacters.ContainsKey(e.Client)) 
        {
            LogoutClient(e.Client);
        }
    }
    private void OnMasterDisconnected()
    {
        masterServer.MessageReceived -= OnMasterServerLoginMessage;
    }

    private void OnMasterConnected()
    {
        masterServer.MessageReceived -= ClientAuthRequest;
        masterServer.MessageReceived += OnMasterServerLoginMessage;
    }
    #endregion

    public ServerCharData GetClientData(IClient client) 
    {
        if(loggedInCharacters.TryGetValue(client,out var charID))
        {
            if(charData.TryGetValue(charID,out var res)) 
            {
                return res;
            }
        }
        return null;
    }

    #region LogoutClient

    public void LogoutClient(IClient client) 
    {
        if(loggedInCharacters.TryGetValue(client,out var res)) 
        {
            LogoutClient(client, res);
        }
    }

    public void LogoutClient(IClient client, string charID)
    {
        if (loggedInCharacters.ContainsKey(client)) 
        {
            OnClientLoggedOut?.Invoke(client);
            RemoveCharacter(client, charID);
            charData.Remove(charID);
        }
        if (client.ConnectionState != ConnectionState.Disconnecting || client.ConnectionState != ConnectionState.Disconnected) 
        {
            client.Disconnect();
        }
    }

    #endregion

    #region LoginClient

    private void LoginClient(IClient client,string charID)
    {
        client.MessageReceived -= OnMasterServerLoginMessage;
        client.MessageReceived += OnLogoutRequest;
        if (loggedInCharacters.ContainsKey(client) || loggedInCharactersByID.ContainsKey(charID)) 
        {
            LogoutClient(client, charID);
            return;
        }
        AddCharacter(client, charID);
        sessionTokens.Remove(charID);
        OnClientLoggedIn?.Invoke(client);
    }

    #endregion

    #region Client Requests

    private void ClientAuthRequest(object sender, MessageReceivedEventArgs e)
    {
        if (e.Tag != (ushort)ClientTags.LoginRequest)
        {
            return;
        }
        using (var message = e.GetMessage())
        {
            using (var reader = message.GetReader())
            {

                var loginData = reader.ReadSerializable<LoginRequestData>();

                if (sessionTokens.TryGetValue(loginData.CharID, out var token))
                {
                    if (loginData.Token == token)
                    {
                        e.Client.SendMessageNoContent((ushort)ClientTags.LoginRequestAccepted, SendMode.Reliable);
                        LoginClient(e.Client, loginData.CharID);
                        return;
                    }
                }
                e.Client.SendMessageNoContent((ushort)ClientTags.LoginRequestDenied, SendMode.Reliable);
            }
        }
    }
    private void OnLogoutRequest(object sender, MessageReceivedEventArgs e)
    {
        if(e.Tag != (ushort)ClientTags.LogoutRequest) 
        {
            return;
        }
        Debug.Log($"{e.Client.ID} has requested to be logged out");
        LogoutClient(e.Client);
    }

    #endregion

    #region MasterServerLoginMessage Handler

    private void OnMasterServerLoginMessage(object sender, MessageReceivedEventArgs e)
    {
        if(e.Tag != (ushort)MasterServerReplyTags.CharacterLoggedIn)
        {
            return;
        }
        using (var message = e.GetMessage())
        {
            using (var reader = message.GetReader())
            {

                var messID = reader.ReadUInt16();
                var data = reader.ReadSerializable<ServerCharData>();
                ushort sessKey;
                if (sessionTokens.ContainsKey(data.charID))
                {
                    sessKey = sessionTokens[data.charID];
                }
                else
                {
                    sessKey = (ushort)UnityEngine.Random.Range(10, ushort.MaxValue);
                }
                sessionTokens[data.charID] = sessKey;
                charData[data.charID] = data;


                Debug.Log($"Client {data.charID} has logged in with session key {sessKey}");

                using (var writer = DarkRiftWriter.Create())
                {

                    writer.Write(messID);
                    writer.Write(new ServerAuthReply { sessToken = sessKey });

                    using (var reply = Message.Create((ushort)MasterServerReplyTags.CharacterLoginAck, writer))
                    {
                        e.Client.SendMessage(reply, SendMode.Reliable);
                    }
                }
            }
        }
    }

    #endregion

    private void OnDisable()
    {
        masterServerManager.OnMasterConnected -= OnMasterConnected;
        masterServerManager.OnMasterDisconnected -= OnMasterDisconnected;
        clientManager.ClientConnected -= RegisterClient;
        clientManager.ClientDisconnected -= UnRegisterClient;
    }

    #region Helpers

    private void AddCharacter(IClient client, string charID)
    {
        loggedInCharacters[client] = charID;
        loggedInCharactersByID[charID] = client;
    }
    private void RemoveCharacter(IClient client, string charID)
    {
        loggedInCharacters.Remove(client);
        loggedInCharactersByID.Remove(charID);
    }

    #endregion
}
