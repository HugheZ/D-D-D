﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerRunManager : MonoBehaviour {
    const int ROOM_COUNT = 3;

    public List<GameObject> possibleRoomList;
    public GameObject bossRoom;
    public GameObject boss;
    bool bossSpawned;
    public Transform zero;
    public Transform p1Space;
    public Transform p2Space;
    public Transform p3Space;
    public Transform p4Space;
    public List<Vector3> playerSpawns;
    public Camera p1camera, p2camera, p3camera, p4camera;
    System.Random rnd;
    List<GameObject> runOrder;
    int p1CurRoom = 0;
    int p2CurRoom = 0;
    int p3CurRoom = 0;
    int p4CurRoom = 0;
    GameObject p1r1;
    GameObject p2r1;
    GameObject p3r1;
    GameObject p4r1;

    //stuff for Singleton
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
        rnd = new System.Random();
        runOrder = new List<GameObject>();
        for(int rc = 0; rc < ROOM_COUNT; rc++)
        {
            int traps = rnd.Next(3,6);
            GameObject startRoom = possibleRoomList[rnd.Next(0, possibleRoomList.Count)];
            startRoom.transform.GetChild(traps).gameObject.SetActive(true);
            runOrder.Add(startRoom);
        }
        
        p1r1 = Instantiate(runOrder[0], p1Space);
        p2r1 = Instantiate(runOrder[0], p2Space);
        p3r1 = Instantiate(runOrder[0], p3Space);
        p4r1 = Instantiate(runOrder[0], p4Space);

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
    public void NextRoom(GameObject player)
    {
        player.GetComponent<CollisionHandler>().ToggleInteractivity(false);
        int playerNum = 0;
        switch (playerNum)
        {
            case 1:
                Destroy(p1r1);
                p1CurRoom++;
                if (p1CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p1r1 = Instantiate(runOrder[p1CurRoom], p1Space);
                    player.transform.position = playerSpawns[0];
                }
                break;
            case 2:
                Destroy(p2r1);
                p2CurRoom++;
                if (p2CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p2r1 = Instantiate(runOrder[p2CurRoom], p2Space);
                    player.transform.position = playerSpawns[1];
                }
                break;
            case 3:
                Destroy(p3r1);
                p3CurRoom++;
                if (p3CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p3r1 = Instantiate(runOrder[p3CurRoom], p3Space);
                    player.transform.position = playerSpawns[2];
                }
                break;
            default:
                Destroy(p4r1);
                p4CurRoom++;
                if (p4CurRoom >= runOrder.Count)
                {
                    //player reaches boss room
                }
                else
                {
                    p4r1 = Instantiate(runOrder[p4CurRoom], p4Space);
                    player.transform.position = playerSpawns[3];
                }
                break;
        }
        player.GetComponent<CollisionHandler>().ToggleInteractivity(true);
        
    }
    private bool PlayerInRoom()
    {
        //TODO: check if a player reaches boss room
        return true;
    }
    public void PlayerDied()
    {
        //invoke show
        //Invoke("ShowEnd", timeToShowEnd);
        //update final score
        //finalScore.text = string.Format("Rooms Cleared:\n{0}", roomsCleared);
    }

    /// <summary>
    /// Invokable method to show the end screen
    /// </summary>
    private void ShowEnd()
    {
        //endPlayer.mute = true;
        //endPlayer.PlayOneShot(endAudio);
        //gameOverUI.SetActive(true);
    }
    public void BackToMenu()
    {
        //endPlayer.PlayOneShot(click);
        SceneManager.LoadScene("MainMenu");
    }
}
