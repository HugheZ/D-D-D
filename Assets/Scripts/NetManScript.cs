using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManScript : NetworkManager
{
    public Dictionary<int, GameObject> playerMap;
    NetworkManager myNetMan;

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
    }
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
    }


}
