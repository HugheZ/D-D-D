using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManScript : NetworkManager
{
    public Dictionary<int, GameObject> playerMap;
    public Transform p1startPt;
    public Transform p2startPt;
    public Transform p3startPt;
    public Transform p4startPt;
    NetworkManager myNetMan;
    MultiplayerRunManager mrm;
    int p1ConnId;

    //list of player animatiors
    public List<RuntimeAnimatorController> controllers;

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
        Vector2 spawnAt = new Vector2();
        switch (playerMap.Keys.Count)
        {
            case 1:
                spawnAt = p2startPt.position;
                break;
            case 2:
                spawnAt = p3startPt.position;
                break;
            case 3:
                spawnAt = p4startPt.position;
                break;
            default:
                spawnAt = p1startPt.position;
                break;
        }
        var player = Instantiate(playerPrefab, spawnAt, Quaternion.identity);
        if (playerMap.Keys.Count == 0)
            p1ConnId = playerControllerId;
        //edit player
        playerMap.Add(conn.connectionId, player);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        mrm.numPlayers = playerMap.Count;
        print(playerMap.Keys.Count);
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

        //change animator
        //if (playerMap.Keys.Count <= controllers.Count)
        //{
        //    player.GetComponent<PlayerIdentityController>().index = playerMap.Keys.Count - 1;
        //}

    }
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        mrm.numPlayers--;
        mrm.updateCamera();
    }

    /// <summary>
    /// Disconnects the player
    /// </summary>
    /// <param name="isServer">Whether the caller is a host or client</param>
    public void Disconnect(bool isServer)
    {
        if (isServer) StopHost();
        else StopClient();
    }
}
