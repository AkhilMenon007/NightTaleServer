using DarkRift;
using DarkRift.Server;
using MasterServer.DarkRift.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MasterServerManager : MonoBehaviour
{
    public static MasterServerManager instance = null;
    public IClient masterServer { get; private set; } = null;

    private Dictionary<IClient, RSAParameters> masterClientKeys = new Dictionary<IClient, RSAParameters>();


    private string passCode = "YAREYAREDAZE!";
    public Action OnMasterDisconnected { get; set; }
    public Action OnMasterConnected { get; set; }

    private IClientManager clientManager => ServerManager.Instance.Server.ClientManager;


    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(this);
            return;
        }
    }


    private void OnEnable()
    {
        clientManager.ClientConnected += OnClientConnected;
        clientManager.ClientDisconnected += OnClientDisconnected;
    }
    private void OnDisable()
    {
        clientManager.ClientConnected -= OnClientConnected;
        clientManager.ClientDisconnected -= OnClientDisconnected;
    }


    private void OnClientConnected(object sender, ClientConnectedEventArgs e)
    {
        e.Client.MessageReceived += OnMessageReceived;
    }
    private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= OnMessageReceived;

        if(e.Client == masterServer)
        {
            MasterServerDisconnected();
        }
    }

    private void MasterServerDisconnected()
    {
        OnMasterDisconnected?.Invoke();
        clientManager.ClientConnected += OnClientConnected;
        masterServer = null;
    }

    private void OnMasterServerIdentified(IClient masterServerClient) 
    {
        masterServer = masterServerClient;
        clientManager.ClientConnected -= OnClientConnected;
        foreach (var client in masterClientKeys.Keys)
        {
            client.MessageReceived -= OnMessageReceived;
        }
        masterServerClient.SendMessage((ushort)MasterServerAuthReplies.Success, (ushort)MasterServerNoReplyTags.Acknowledge, SendMode.Reliable);
        OnMasterConnected?.Invoke();
        masterClientKeys.Clear();
        
    }
    private void OnMasterServerIdentificationFail(IClient client)
    {
        client.MessageReceived -= OnMessageReceived;
        masterClientKeys.Remove(client);

        client.SendMessage((ushort)MasterServerAuthReplies.Fail, (ushort)MasterServerNoReplyTags.Acknowledge, SendMode.Reliable);

        client.Disconnect();
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch ((MasterServerNoReplyTags)e.Tag) 
        {
            case MasterServerNoReplyTags.AuthRequest:
                SendPublicKey(sender, e);
                break;
            case MasterServerNoReplyTags.Password:
                Authorize(sender, e);
                break;
        }
    }

    private void SendPublicKey(object sender, MessageReceivedEventArgs e)
    {
        Cryptography.GenerateRSAKeys(out var privateKey, out var publicKey);
        masterClientKeys[e.Client] = privateKey;

        using(var writer = DarkRiftWriter.Create()) 
        {
            writer.Write(Cryptography.RsaKeyToString(publicKey));
            using(var message = Message.Create((ushort)MasterServerNoReplyTags.PublicKey, writer)) 
            {
                e.Client.SendMessage(message, SendMode.Reliable);
            }
        }
    }

    private void Authorize(object sender, MessageReceivedEventArgs e)
    {
        using(var message = e.GetMessage()) 
        {
            using(var reader = message.GetReader()) 
            {
                using(var decryptedMsg = reader.DecryptReaderRSA(masterClientKeys[e.Client])) 
                {
                    var passCode = decryptedMsg.ReadString();
                    Debug.Log(passCode);
                    if(passCode == this.passCode) 
                    {
                        OnMasterServerIdentified(e.Client);
                    }
                    else 
                    {
                        OnMasterServerIdentificationFail(e.Client);
                    }
                }
            }
        }
    }


}
