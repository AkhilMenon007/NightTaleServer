using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using FYP.Shared;
using FYP.Shared.Login;
using System;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    [Header("References")]
    public XmlUnityServer XmlServer = null;

    public DarkRiftServer Server { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        XmlServer.Create();
        Server = XmlServer.Server;

        DontDestroyOnLoad(this);
    }

}
