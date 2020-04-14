using DarkRift;
using DarkRift.Server;
using MasterServer.DarkRift.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MasterServerManager : MonoBehaviour
{
    public static MasterServerManager instance = null;
    public IClient masterServer { get; private set; } = null;

    private Dictionary<IClient, byte[]> masterClientKeys = new Dictionary<IClient, byte[]>();


    private string passCode = "Tpu3pHh5/dXrZZaghUwcz7kxEiVO1yuNHrDC7bu2J4A=";
    public Action OnMasterDisconnected { get; set; }
    public Action OnMasterConnected { get; set; }

    private IClientManager clientManager => ServerManager.Instance.Server.ClientManager;

    private byte[] passCodeBytes;

    private string commonMessage;

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
        passCodeBytes = Convert.FromBase64String(passCode);
        var random = new System.Random(DateTime.Now.Millisecond);
        var bytes = new byte[32];
        random.NextBytes(bytes);
        commonMessage = Convert.ToBase64String(bytes);
        //Debug.Log(commonMessage);
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

        Debug.Log($"Identified Client {masterServerClient.ID} as master client");
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

    private async void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        switch ((MasterServerNoReplyTags)e.Tag) 
        {
            case MasterServerNoReplyTags.AuthRequest:
                await SendPublicKey(sender, e);
                break;
            case MasterServerNoReplyTags.Password:
                await Authorize(sender, e);
                break;
        }
    }

    private Task SendPublicKey(object sender, MessageReceivedEventArgs e)
    {
        var key = Cryptography.GenerateAESKey();
        masterClientKeys[e.Client] = key;

        using(var writer = DarkRiftWriter.Create()) 
        {
            writer.Write(Convert.ToBase64String(key));
            writer.Write(commonMessage);
            using (var encryptedWriter = writer.EncryptWriterAES(passCodeBytes))
            {
                using (var message = Message.Create((ushort)MasterServerNoReplyTags.PublicKey, encryptedWriter))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
        return Task.CompletedTask;
    }

    private Task Authorize(object sender, MessageReceivedEventArgs e)
    {
        using(var message = e.GetMessage()) 
        {
            using(var decryptedMsg = message.GetReader().DecryptReaderAES(masterClientKeys[e.Client])) 
            {
                var passCode = decryptedMsg.ReadString();
                if(string.Equals(passCode,commonMessage,StringComparison.Ordinal)) 
                {
                    OnMasterServerIdentified(e.Client);
                    Debug.Log("Master identified..");
                }
                else 
                {
                    OnMasterServerIdentificationFail(e.Client);
                    Debug.Log("Master identification failed..");
                }
            }
        }
        return Task.CompletedTask;
    }


}
