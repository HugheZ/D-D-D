using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerRunManager : NetworkBehaviour {
    const int ROOM_COUNT = 3;

    public List<GameObject> possibleRoomList;
    public GameObject bossRoom;
    public GameObject boss;
    public GameObject player1, player2, player3, player4;
    bool bossSpawned;
    public Transform zero;
    public Transform p1Space;
    public Transform p2Space;
    public Transform p3Space;
    public Transform p4Space;
    public List<Vector3> playerSpawns;
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
    public Camera p1camera, p2camera, p3camera, p4camera, main;
    public Canvas progressCanvas;
    public Image sideToSide;
    public Image topToBottom;
    public Image player1progress, player2progress, player3progress, player4progress;
    bool gameStarted;
    public Canvas holdingRoomCanvas;
    Dictionary<int, GameObject> playerMapCopy;

    public int numPlayers = 0;

    public ProgressDiamondScript pDiamondScript;

    public SyncListFloat progresses;

    //Keeps scores for all players
    Dictionary<int, int> scoreTable;

    //End Game UI variables
    public Canvas endGameHud;
    public Text endGameWinnerText;
    public Image endGameWinnerImage;
    public Text endGameSalute;
    //Whether the game is over
    bool gameOver;

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
        gameStarted = false;

        //Unfill all progress
        player1progress.fillAmount = 0;
        player2progress.fillAmount = 0;
        player3progress.fillAmount = 0;
        player4progress.fillAmount = 0;

        //Initalize Cameras
        main.rect = new Rect(0, 0, 1, 1);

        //Deinitialize progress diamond
        progressCanvas.gameObject.SetActive(false);

        pDiamondScript = FindObjectOfType<ProgressDiamondScript>();
        progresses = pDiamondScript.progresses;

        gameOver = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerInRoom() && !bossSpawned && isServer)
        {
            //enable boss ui and such
            if (!BossManager.Instance)
            {
                GameObject bossBoy = Instantiate(boss, zero, true);
                NetworkServer.Spawn(bossBoy);

                Debug.Log("Created new boss, but be aware:\nThis boss creation is not fully featured. Bosses do not link to manager. Game state is undefined.");
            }
            else
            {
                RpcEnableBoss();
            }
            bossSpawned = true;
        }
        if (!gameStarted && isServer && Input.GetKeyDown(KeyCode.R))
            RpcStartGame();
    }

    /// <summary>
    /// Enables the boss on all clients
    /// </summary>
    [ClientRpc]
    void RpcEnableBoss()
    {
        print("Received instruction to enable boss");
        BossManager.Instance.EnableBoss();
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////
    /// </summary>
    
    [ClientRpc]
    public void RpcStartGame()
    {
        gameStarted = true;
        holdingRoomCanvas.enabled = false;
        print("Game start!");        
        NetManScript netman = NetManScript.Instance;
        playerMapCopy = netman.playerMap;
        var e = netman.playerMap.GetEnumerator();
        numPlayers = (netman.playerMap.Keys.Count);
        for (int i = 0; i < netman.playerMap.Keys.Count; i++)
        {            
            e.MoveNext();
            e.Current.Value.gameObject.transform.position = playerSpawns[i];
        }
        InitializeScoreTable();
        updateCamera();
    }

    int GetPlayerSpawnPoint(GameObject player)
    {
        NetworkConnection tpPlayer = player.GetComponentInParent<NetworkIdentity>().connectionToClient;
        int playerNum = 0;
        var e = playerMapCopy.Keys.GetEnumerator();
        for (int i = 0; i < playerMapCopy.Keys.Count; i++)
        {
            e.MoveNext();
            if (e.Current == tpPlayer.connectionId)
                playerNum = i;
        }
        return playerNum;
    }

    public Vector3 GetRespawnPoint(GameObject player)
    {
        return playerSpawns[GetPlayerSpawnPoint(player)];
    }


    public void NextRoom(GameObject player)
    {
        player.GetComponent<CollisionHandler>().ToggleInteractivity(false);
        int playerNum = GetPlayerSpawnPoint(player);
        switch (playerNum)
        {
            case 0:
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
            case 1:
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
            case 2:
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
        if(numPlayers > 1)
        {
            float progress = (1.0f / (float)ROOM_COUNT) / 2.0f;
            pDiamondScript.ChangeProgress(playerNum - 1, progress);
            RpcProgressUpdate();
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
        if (isServer)
        {
            int deadPlayers = 0;

            List<int> keys = new List<int>(scoreTable.Keys);

            //loop over players in score table, check NetMan for their objects, and see if they are dead, associate that player's score with 0
            foreach (int ID in keys)
            {
                if (NetManScript.Instance.playerMap[ID].GetComponent<HealthSystem>().health <= 0)
                {
                    //player has 0 HP, so is dead, so we give them -1 points and update dead player count
                    deadPlayers++;
                    scoreTable[ID] = -1;
                }
            }

            //if all players are dead, end the game
            if (deadPlayers == scoreTable.Keys.Count)
            {
                RpcEndGame(false, -1);
            }
        }
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

    [ClientRpc]
    private void RpcProgressUpdate()
    {
        if (isServer)
        {
            progresses = pDiamondScript.progresses;
        }
    }

    public void updateCamera()
    {
        if(numPlayers == 1)
        {
            //Deinitialize progress diamond
            progressCanvas.gameObject.SetActive(false);

            player1.SetActive(true);
            if(player2 != null)
            {
                player2.SetActive(false);
            }
            if (player3 != null)
            {
                player3.SetActive(false);
            }
            if (player4 != null)
            {
                player4.SetActive(false);
            }

            p1camera.rect = new Rect(0, 0, 1, 1);
            main.rect = new Rect(0, 0, 0, 0);
            if(p2camera != null)
            {
                p2camera.rect = new Rect(0, 0, 0, 0);
            }
           

        }else if(numPlayers == 2)
        {
            //Initialize progress diamond
            progressCanvas.gameObject.SetActive(true);

            player1.SetActive(true);
            player2.SetActive(true);
            if (player3 != null)
            {
                player3.SetActive(false);
            }
            if (player4 != null)
            {
                player4.SetActive(false);
            }

            //Insert camera splits
            topToBottom.gameObject.SetActive(true);
            sideToSide.gameObject.SetActive(false);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, 0, .5f, 1);
            p2camera.rect = new Rect(.5f, 0, .5f, 1);
            main.rect = new Rect(0, 0, 0, 0);
        }
        else if(numPlayers == 3)
        {
            //Initialize progress diamond
            progressCanvas.gameObject.SetActive(true);

            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
            if(player4 != null)
            {
                player4.SetActive(false);
            }
            

            //Insert camera splits
            topToBottom.gameObject.SetActive(true);
            sideToSide.gameObject.SetActive(true);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, .5f, .5f, .5f);
            p2camera.rect = new Rect(.5f, .5f, .5f, .5f);
            p3camera.rect = new Rect(0, 0, .5f, .5f);
            main.rect = new Rect(.5f, 0, .5f, .5f);
        }
        else if(numPlayers == 4)
        {
            //Initialize progress diamond
            progressCanvas.gameObject.SetActive(true);

            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
            player4.SetActive(true);

            //Insert camera splits
            topToBottom.gameObject.SetActive(true);
            sideToSide.gameObject.SetActive(true);

            //Instantiate camera sizes
            p1camera.rect = new Rect(0, .5f, .5f, .5f);
            p2camera.rect = new Rect(.5f, .5f, .5f, .5f);
            p3camera.rect = new Rect(0, 0, .5f, .5f);
            p4camera.rect = new Rect(.5f, 0, .5f, .5f);
            main.rect = new Rect(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void InitializeScoreTable()
    {
        //Initialize score table to current state on game start, give each player score of 0
        scoreTable = new Dictionary<int, int>();
        foreach(int key in NetManScript.Instance.playerMap.Keys)
        {
            scoreTable.Add(key, 0);
        }
    }

    /// <summary>
    /// Called when a player strikes the boss, awards points to the player
    /// </summary>
    public void AwardPointsByID(int ID, float damage)
    {
        //if the ID is in the map, increment score
        if (scoreTable.ContainsKey(ID))
        {
            scoreTable[ID] += Mathf.FloorToInt(damage);
            string ret = "";
            foreach (KeyValuePair<int, int> pair in scoreTable) ret += "[" + pair.Key + ": " + pair.Value + "] ";
            print(ret);
        }
        //else print an error log to the console
        else Debug.LogError("Key awarded points, but does not exist in score table: " + ID);

    }

    /// <summary>
    /// Called when the boss is defeated, ends the game
    /// Winner = most damage dealt to boss
    /// </summary>
    public void BossDefeated()
    {
        if (isServer)
        {
            // find highest scoring player
            int winner = -1;
            int score = 0;
            foreach(KeyValuePair<int, int> entry in scoreTable)
            {
                if (entry.Value > score) winner = entry.Key;
            }

            // tell everyone about it
            RpcEndGame(true, winner);
        }
    }

    /// <summary>
    /// Ends the game for all players
    /// </summary>
    /// <param name="bossDefeated">Whether the game ended in a boss defeat or not</param>
    [ClientRpc]
    void RpcEndGame(bool bossDefeated, int playerIfWon)
    {
        if (!gameOver)
        {
            gameOver = true;

            if (bossDefeated)
            {
                print("Boss died, player wins: " + playerIfWon);
                //if the boss died, find out the winning player's image and plaster it on the end game UI
                if(playerIfWon != -1)
                {
                    //set player if we have a valid index
                    endGameWinnerImage.sprite = NetManScript.Instance.playerMap[playerIfWon].GetComponent<SpriteRenderer>().sprite;
                    //set other fields
                    endGameWinnerText.text = string.Format("Winner!\n{0}", playerIfWon);
                    endGameSalute.text = "Congratulations, noble dwarf.\n\nYou have bested the dungeon and your traitorous kin.\nReturn now, live in luxury, and enjoy your honor!";
                }
                else
                {
                    endGameWinnerText.text = "Winner!";
                    endGameSalute.text = "Contragulations, unknown dwarf.\n\nYou may be a ghost to us, but you shall go down in the annals of history.\nThe rest may fight over who the winner was.";
                }

                endGameHud.gameObject.SetActive(true);
            }
            else
            {
                //boss did not die, everyone else did, simply keep the base text and present the menu
                endGameHud.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Disconnects the player from the game
    /// </summary>
    public void Disconnect()
    {
        NetManScript.Instance.Disconnect(isServer);
        SceneManager.LoadScene("MainMenu");
    }
}
