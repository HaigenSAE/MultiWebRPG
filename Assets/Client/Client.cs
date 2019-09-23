using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    //const
    private const int MAX_CONNECTIONS = 100;
    private string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 8999;
    private const int SERVER_WEB_PORT = 8998;
    private const int BUFFER_SIZE = 1024;

    //Channels
    private int reliableChannelId;      //Purchase an item
    private int unreliableChannelId;    //Updating movement of other players

    //Host
    private int hostId;
    private int connectionId;

    //Logic
    private byte error;
    private byte[] buffer = new byte[BUFFER_SIZE];
    private bool isConnected;

    void Start()
    {
        GlobalConfig config = new GlobalConfig();
        NetworkTransport.Init(config);

        //Host Topology
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannelId = cc.AddChannel(QosType.Reliable);
        unreliableChannelId = cc.AddChannel(QosType.Unreliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTIONS);

        //Connecting to Host
        hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL
        //WebGL Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_WEB_PORT, 0, out error);
#else
        //Standalone Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, SERVER_PORT, 0, out error);
#endif
    }
}
