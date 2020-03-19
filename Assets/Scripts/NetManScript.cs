using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManScript : NetworkManager
{
    public Dictionary<int, GameObject> playerMap;
    NetworkManager myNetMan;
    MultiplayerRunManager mrm;

    private static NetManScript _instance = null;
    public static NetManScript Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }
    // Use this for initialization
    void Start()
    {
        myNetMan = GetComponent<NetworkManager>();
        playerMap = new Dictionary<int, GameObject>();
        mrm = MultiplayerRunManager.Instance;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //edit player
        playerMap.Add(conn.connectionId, player);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        mrm.numPlayers = playerMap.Count;
        print(playerMap.Count);
        if (mrm.numPlayers == 1)
        {
            mrm.player1 = player.gameObject;
            mrm.p1camera = player.GetComponentInChildren<Camera>();
        }
        else if (mrm.numPlayers == 2)
        {
            mrm.player2 = player.gameObject;
            mrm.p2camera = player.GetComponentInChildren<Camera>();
        }
        else if (mrm.numPlayers == 3)
        {
            mrm.player3 = player.gameObject;
            mrm.p3camera = player.GetComponentInChildren<Camera>();
        }
        else if (mrm.numPlayers == 4)
        {
            mrm.player4 = player.gameObject;
            mrm.p4camera = player.GetComponentInChildren<Camera>();
        }
        mrm.updateCamera();
        


    }
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        mrm.numPlayers--;
        mrm.updateCamera();
    }

}
