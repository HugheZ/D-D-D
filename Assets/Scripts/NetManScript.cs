using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManScript : NetworkManager {
    bool p1Ready = false;
    public GameObject dwarfPrefab;
    //NetworkManager myNetMan;

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
    void Start () {
        //myNetMan = GetComponent<NetworkManager>();

    }
	
	// Update is called once per frame
	void Update () {
        if (p1Ready)
        {
            p1Ready = false;
            playerPrefab = dwarfPrefab;
            ServerChangeScene("MultiplayerRun");
        }
	}
    public void Player1Ready()
    {
        p1Ready = true;
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
    }
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
    }


}
