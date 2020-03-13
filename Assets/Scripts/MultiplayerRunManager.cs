using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerRunManager : MonoBehaviour {

    public List<GameObject> possibleRoomList;
    public GameObject bossRoom;
    public GameObject boss;
    bool bossSpawned;
    public Transform zero;
    public Transform p1Space;
    public Transform p2Space;
    public Transform p3Space;
    public Transform p4Space;
    public List<Transform> playerSpawns;
    public Camera p1camera, p2camera, p3camera, p4camera;
    System.Random rnd;
    

	// Use this for initialization
	void Start () {
        rnd = new System.Random();
        GameObject startRoom = possibleRoomList[rnd.Next(0,possibleRoomList.Count)];
        GameObject p1r1 = Instantiate(startRoom, p1Space);
        GameObject p2r1 = Instantiate(startRoom, p2Space);
        GameObject p3r1 = Instantiate(startRoom, p3Space);
        GameObject p4r1 = Instantiate(startRoom, p4Space);

        bossSpawned = false;

        //Instantiate camera sizes
        p1camera.rect = new Rect(0, 0, .5f, .5f);
        p2camera.rect = new Rect(.5f, 0, .5f, .5f);
        p3camera.rect = new Rect(0, .5f, .5f, .5f);
        p4camera.rect = new Rect(.5f, .5f, .5f, .5f);

    }
	
	// Update is called once per frame
	void Update () {
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
}
