using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetManScript : NetworkManager
{
    public Dictionary<int, GameObject> playerMap;
    public Transform p1startPt;
    NetworkManager myNetMan;
    MultiplayerRunManager mrm;
    int p1ConnId;

    //list of player animatiors
    public List<RuntimeAnimatorController> controllers;

    //lsit of all approved player names
    public List<string> playerNames;

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
        //base.OnServerAddPlayer(conn, playerControllerId);
        Vector2 spawnAt = p1startPt.position;
        var player = Instantiate(playerPrefab, spawnAt, Quaternion.identity);
        if (playerMap.Keys.Count == 0)
            p1ConnId = playerControllerId;
        //edit player
        LinkObjectToID(player, conn.connectionId);

        NetworkServer.Spawn(player);

        ////add to connection
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        print(playerMap.Keys.Count);

        ////change animator
        ////if (playerMap.Keys.Count <= controllers.Count)
        ////{
        ////    player.GetComponent<PlayerIdentityController>().index = playerMap.Keys.Count - 1;
        ////}
    }

    /// <summary>
    /// Links a gameobject with a particular ID
    /// </summary>
    /// <param name="player">The game object to link</param>
    /// <param name="ID">That player's connection ID</param>
    public void LinkObjectToID(GameObject player, int ID)
    {
        playerMap.Add(ID, player);
        PlayerIdentityController identity = player.GetComponent<PlayerIdentityController>();
        LocalPlayerIdentity localIdentity = player.GetComponent<LocalPlayerIdentity>();
        if (identity)
        {
            identity.playerAnimIndex = playerMap.Keys.Count - 1;
            string name = playerNames.Count > 0 ? playerNames[Random.Range(0, playerNames.Count)] : "Player " + playerMap.Keys.Count;
            identity.name = name;
            identity.UID = ID;
        }
        else
        {
            localIdentity.playerAnimIndex = playerMap.Keys.Count - 1;
            localIdentity.name = "Player 2";
            localIdentity.UID = ID;
        }

        MultiplayerRunManager.Instance.numPlayers = playerMap.Keys.Count;
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        MultiplayerRunManager.Instance.numPlayers = playerMap.Keys.Count;
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
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        SceneManager.LoadScene("MainMenu");
    }
    public void StartUpHost()
    {
        NetworkManager.singleton.StopHost();
        SetPort();
        NetworkManager.singleton.serverBindToIP = true;
        //NetworkManager.singleton.serverBindAddress = "localhost";//Network.player.ipAddress;
        //SetIPAddress();
        NetworkManager.singleton.StartHost();
    }
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7779;
    }

}
