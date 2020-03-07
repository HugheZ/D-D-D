using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerRunManager : MonoBehaviour {

    GameObject p1room;
    GameObject p2room;
    GameObject p3room;
    GameObject p4room;
    public GameObject bossRoom;
    public GameObject boss;
    bool bossSpawned;
    bool playersSpawned = false;
    public Transform zero;
    public Transform p1Space;
    public Transform p2Space;
    public Transform p3Space;
    public Transform p4Space;
    public List<Vector2> playerSpawns;
    NetManScript netman;

    private static MultiplayerRunManager _instance = null;
    public static MultiplayerRunManager Instance
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

        netman = NetManScript.Instance;
        SetTrapsEnabled();
        SpawnPlayers();

        if (p1room == null)
        {
            p1room = GameObject.FindGameObjectWithTag("P1Room");            
            p1room.SetActive(false);
            p1room.transform.position = new Vector2(0, 0);
        }
        /*if (p2room == null)
        {
            p2room = GameObject.FindGameObjectWithTag("P2Room");
            p2room.SetActive(false);
        }
        if (p3room == null)
        {
            p3room = GameObject.FindGameObjectWithTag("P3Room");
            p3room.SetActive(false);
        }
        if (p4room == null)
        {
            p4room = GameObject.FindGameObjectWithTag("P4Room");
            p4room.SetActive(false);
        }*/

        //start in rooms
        GameObject p1r1 = Instantiate(p1room, p1Space);
        GameObject p2r1 = Instantiate(p1room, p2Space);
        GameObject p3r1 = Instantiate(p1room, p3Space);
        GameObject p4r1 = Instantiate(p1room, p4Space);
        
        p1r1.SetActive(true);
        p2r1.SetActive(true);
        p3r1.SetActive(true);
        p4r1.SetActive(true);       

        bossSpawned = false;

    }
	
	// Update is called once per frame
	void Update () {
        p1room.SetActive(false);
        SetTrapsEnabled();
        if (!playersSpawned)
            SpawnPlayers();

        if (PlayerInRoom() && bossSpawned)
        {
            Instantiate(boss, zero);
            bossSpawned = true;
            //TODO: enable boss ui and such
        }
	}
    private bool PlayerInRoom()
    {
        //TODO: check if a player reaches boss room
        return true;
    }
    private void SetTrapsEnabled()
    {
        ArrowTrapScript[] arrowTraps = (ArrowTrapScript[])FindObjectsOfType(typeof(ArrowTrapScript));
        foreach (ArrowTrapScript arrowTrap in arrowTraps)
            arrowTrap.SetEnabled();
        SawbladeScript[] sawblades = (SawbladeScript[])FindObjectsOfType(typeof(SawbladeScript));
        foreach (SawbladeScript sawblade in sawblades)
            sawblade.FlipActive();
    }
    private void SpawnPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int index = 0; index < players.Length; index++)
        {
            GameObject player = players[index];
            player.transform.position = playerSpawns[index];
        }
        playersSpawned = true;
    }
}
