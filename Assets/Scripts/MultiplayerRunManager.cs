using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerRunManager : NetworkBehaviour {
    const int ROOM_COUNT = 3;

    AcheivementScript achievements;

    public List<GameObject> possibleRoomList;
    public GameObject bossRoom; //room for the end-game
    public GameObject boss; //boss object
    public GameObject goldRoom; //gold room and zone
    public GameObject player1, player2, player3, player4;
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
    public Button menuButton;
    public Camera startCamera;

    public Vector3 player1RespawnPoint, player2RespawnPoint;

    public GameObject p1HoldingRoom, p2HoldingRoom, p3HoldingRoom, p4HoldingRoom;
    public GameObject p1Block, p2Block, p3Block, p4Block;

    public bool isLocalMultiplayerGame = false;

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

    /// Temporary sprint 2 fields

    public GameObject dungeon;
    public GameObject containingDoor;


    /// 
    bool endGameStarted; // has the end of the game begun?
    public bool endGameChoice; //choice in endgame, true for boss and false for gold

    float gameStartTime;

    [SerializeField]
    GameObject localMultiplayerPrefab; //prefab for local multiplayer
    int additionalLocalPlayers = 0;
    bool awaitingLocalJoin = false;

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
        /*for(int rc = 0; rc < ROOM_COUNT; rc++)
        {
            int traps = rnd.Next(3,6);
            GameObject startRoom = possibleRoomList[rnd.Next(0, possibleRoomList.Count)];
            for(int i = 3; i< 6; i++)
            {
                startRoom.transform.GetChild(i).gameObject.SetActive(false);
            }
            startRoom.transform.GetChild(traps).gameObject.SetActive(true);
            runOrder.Add(startRoom);
        }*/
        
        /*p1r1 = Instantiate(runOrder[0], p1Space);
        p2r1 = Instantiate(runOrder[0], p2Space);
        p3r1 = Instantiate(runOrder[0], p3Space);
        p4r1 = Instantiate(runOrder[0], p4Space);*/

        endGameStarted = false;
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

        player1RespawnPoint = p1Space.position;
        player2RespawnPoint = p1Space.position;

        startCamera.enabled = (false);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (!gameStarted && isServer && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyUp(KeyCode.Joystick1Button5)))
        {
            //generate seed for pathing and for end game choice
            int pathSeed = rnd.Next(4);
            int[] roomSeed = { rnd.Next(2, 5) , rnd.Next(2, 5) , rnd.Next(2, 5) , rnd.Next(2, 5) , rnd.Next(2, 5) ,
                rnd.Next(2, 5) , rnd.Next(2, 5) , rnd.Next(2, 5) , rnd.Next(2, 5) };
            endGameChoice = rnd.Next(0,2) == 0 ? false : true;
            if (endGameChoice) print("BOSS TIME");
            else print("GOLD TIME");
            RpcStartGame(pathSeed, roomSeed);
        }

        //if game hasn't started and we are in local multiplayer, poll keyboard input for second player
        if(!gameStarted && isServer && awaitingLocalJoin)
        {
            if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Joystick2Button0))
            {
                SpawnLocalPlayer();
                awaitingLocalJoin = false;
            }
        }
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

    [ClientRpc]
    void RpcEnableGoldRoom()
    {
        print("Received instruction to enable gold room");
        GoldRoomManager.Instance.EnableGoldRoom();
    }

    /// <summary>
    /// Sends an RPC call to enable the game to start on all client machines
    /// </summary>
    /// <param name="pathSeed">The seed used to generate the path seeds</param>
    /// <param name="roomSeed">Individual random seeds used for room generation</param>
    
    [ClientRpc]
    public void RpcStartGame(int pathSeed, int[] roomSeed)
    {
        if (isServer)
        {
            gameStartTime = Time.time;
        }
        gameStarted = true;
        holdingRoomCanvas.enabled = false;
        print("Game start!");
        InitializeScoreTable();
        containingDoor.SetActive(false);
        dungeon.SetActive(true);
        //enable random traps
        for(int i = 0; i < 9; i++)
        {
            Transform thing = dungeon.transform.GetChild(0).GetChild(i);
            GameObject currentRoom = thing.gameObject;
            thing.GetChild(2).gameObject.SetActive(false);
            thing.GetChild(3).gameObject.SetActive(false);
            thing.GetChild(4).gameObject.SetActive(false);
            thing.GetChild(roomSeed[i]).gameObject.SetActive(true);
            
        }
        //determine the path
        //int path = rnd.Next(4);
        if(pathSeed % 2 == 0)
        {
            //go left at first
            dungeon.transform.GetChild(1).gameObject.SetActive(false);
            if(pathSeed == 2)
                dungeon.transform.GetChild(3).gameObject.SetActive(false);
            else
                dungeon.transform.GetChild(4).gameObject.SetActive(false);
        } else
        {
            //go right at first
            dungeon.transform.GetChild(2).gameObject.SetActive(false);
            if (pathSeed == 1)
                dungeon.transform.GetChild(5).gameObject.SetActive(false);
            else
                dungeon.transform.GetChild(6).gameObject.SetActive(false);
        }
    }

    int GetPlayerSpawnPoint(GameObject player)
    {
        NetworkConnection tpPlayer = player.GetComponentInParent<NetworkIdentity>().connectionToClient;
        int playerNum = 0;
        playerMapCopy = NetManScript.Instance.playerMap;
        var e = playerMapCopy.Keys.GetEnumerator();
        for (int i = 0; i < playerMapCopy.Keys.Count; i++)
        {
            e.MoveNext();
            if (e.Current == tpPlayer.connectionId)
                playerNum = i;
        }
        return playerNum;
    }

    public Vector3 GetRespawnPoint(GameObject player, int playerNum)
    {
        if(playerNum == 2)
        {
            return player2RespawnPoint;
        }else
        {
            return player1RespawnPoint;
        }
        
    }

    public void PickEnding()
    {
        if (endGameChoice) //end game true, start boss
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
        }
        else //end game false, start gold
        {
            //enable gold UI and such
            if (!GoldRoomManager.Instance)
            {
                //TODO: instantiate manager
                RpcEnableGoldRoom();
            }
            else
            {
                RpcEnableGoldRoom();
            }
        }

        endGameStarted = true;

    }

    public void NextRoom(GameObject player)
    {
        //enable boss if not yet done
        if (!endGameStarted && gameStarted)
            PickEnding();

        RpcMovePlayerToNextRoom(player);
    }

    [ClientRpc]
    void RpcMovePlayerToNextRoom(GameObject player)
    {
        CollisionHandler col = player.GetComponent<CollisionHandler>();
        CollisionHandlerLocal lcol = player.GetComponent<CollisionHandlerLocal>();
        if (col) col.ToggleInteractivity(false);
        else lcol.ToggleInteractivity(false);

        //teleport to boss
        Vector3 spawnPt = bossRoom.transform.position + new Vector3(0, 8, 0);

        player.transform.position = spawnPt;
        if (col) col.ToggleInteractivity(true);
        else lcol.ToggleInteractivity(true);
    }


    /// <summary>
    /// Records the a dead player event, changes scores, and updates required managers
    /// </summary>
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

            //update end game managers
            UpdateManagersOfDeath();

            //if all players are dead, end the game
            if (deadPlayers == scoreTable.Keys.Count)
            {
                playerMapCopy = NetManScript.Instance.playerMap;
                foreach (int player in playerMapCopy.Keys)
                {
                    GameObject playerObj = playerMapCopy[player];
                    NetworkConnection conn = playerObj.GetComponent<NetworkIdentity>().connectionToClient;
                    TargetEndGame(conn, false, false, -1);
                }
                
            }
        }
    }

    /// <summary>
    /// Tells appropriate end game mangers to update their plaeyrs in-game
    /// </summary>
    void UpdateManagersOfDeath()
    {
        //update gold manager if it is active and running
        if (GoldRoomManager.Instance.enabled) GoldRoomManager.Instance.CheckForDeadPlayers();
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
        if (numPlayers == 1)
        {
            //Deinitialize progress diamond
            progressCanvas.gameObject.SetActive(false);

            player1.SetActive(true);
            if (player2 != null)
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
            if (p2camera != null)
            {
                p2camera.rect = new Rect(0, 0, 0, 0);
            }


        }
        else if (numPlayers == 2)
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
            int counter = 0;
            foreach (KeyValuePair<int, GameObject> player in NetManScript.Instance.playerMap)
            {
                Rect viewport;
                if (counter == 0)
                {
                    viewport = new Rect(0, 0, .5f, 1);
                }
                else
                {
                    viewport = new Rect(.5f, 0, .5f, 1);
                }
                player.Value.GetComponentInChildren<Camera>().rect = viewport;
                counter++;
            }
            main.rect = new Rect(0, 0, 0, 0);
        }
        else if (numPlayers == 3 || numPlayers == 4)
        {
            if (numPlayers == 3)
            {
                //Initialize progress diamond
                progressCanvas.gameObject.SetActive(true);

                player1.SetActive(true);
                player2.SetActive(true);
                player3.SetActive(true);
                if (player4 != null)
                {
                    player4.SetActive(false);
                }


                //Insert camera splits
                topToBottom.gameObject.SetActive(true);
                sideToSide.gameObject.SetActive(true);

                //Instantiate camera main
                main.rect = new Rect(.5f, 0, .5f, .5f);
            }
            else if (numPlayers == 4)
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

                //Instantiate camera main
                main.rect = new Rect(0, 0, 0, 0);
            }

            //Instantiate camera sizes
            int counter = 0;
            foreach (KeyValuePair<int, GameObject> player in NetManScript.Instance.playerMap)
            {
                Rect viewport;
                if (counter == 0)
                {
                    viewport = new Rect(0, .5f, .5f, .5f);
                }
                else if (counter == 1)
                {
                    viewport = new Rect(.5f, .5f, .5f, .5f);
                }
                else if(counter == 2)
                {
                    viewport = new Rect(0, 0, .5f, .5f);
                }
                else
                {
                    viewport = new Rect(.5f, 0, .5f, .5f);
                }
                player.Value.GetComponentInChildren<Camera>().rect = viewport;
                counter++;
            }
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
    /// <param name="ID">ID of the player to award</param>
    /// <param name="points">Number of points to award</param>
    public void AwardPointsByID(int ID, float points)
    {
        //if the ID is in the map, increment score
        if (scoreTable.ContainsKey(ID))
        {
            scoreTable[ID] += Mathf.FloorToInt(points);
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
    public void RoundOver()
    {
        if (isServer)
        {

            playerMapCopy = NetManScript.Instance.playerMap;

            RpcSendTiming(Time.time - gameStartTime);

            // find highest scoring player
            int winner = -1;
            int score = 0;
            foreach(KeyValuePair<int, int> entry in scoreTable)
            {
                if (entry.Value > score) winner = entry.Key;
            }
            

            // tell everyone about it
            foreach (int player in playerMapCopy.Keys)
            {
                print("Telling: " + player);
                GameObject playerObj = playerMapCopy[player];
                NetworkConnection conn = playerObj.GetComponent<NetworkIdentity>().connectionToClient;
                if (player == winner)
                    TargetEndGame(conn, true, true, winner);
                else
                    TargetEndGame(conn, true, false, winner);

                
            }

            //Tell players their score
            
            foreach (KeyValuePair<int, int> s in scoreTable)
            {
                GameObject scoreForPlayer = null;
                int playerScore = 0;
                foreach (KeyValuePair<int, GameObject> player in playerMapCopy)
                {
                    if (s.Key == player.Key)
                    {
                        playerScore = s.Value;
                        scoreForPlayer = player.Value;
                    }
                }
                TargetSendScore(scoreForPlayer.GetComponent<NetworkIdentity>().connectionToClient, playerScore);
            }

        }
    }

    /// <summary>
    /// Ends the game for all players
    /// </summary>
    /// <param name="success">Whether the game ended in a boss defeat / gold taken or not</param>
    [TargetRpc]
    void TargetEndGame(NetworkConnection conn, bool success, bool didWin, int playerIfWon)
    {
        if (!gameOver)
        {
            gameOver = true;

            if (success)
            {

                //find winning player
                Sprite winSprite = null;
                string winName = "Unknown";
                foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
                {
                    PlayerIdentityController pid = p.GetComponent<PlayerIdentityController>();
                    LocalPlayerIdentity lpid = p.GetComponent<LocalPlayerIdentity>();

                    int UID = pid ? pid.UID : lpid.UID;

                    if (UID == playerIfWon)
                    {
                        winName = pid ? pid.name : lpid.name;
                        winSprite = p.GetComponent<SpriteRenderer>().sprite;
                        break;
                    }
                }

                print("Boss died, player wins: " + playerIfWon);

                //we know name and sprite, set it
                endGameWinnerText.text = string.Format("Winner!\n{0}", winName);
                endGameWinnerImage.sprite = winSprite;

                //display appropriate salute depending on if we won or not
                if (didWin)
                {
                    endGameSalute.text = "Congratulations, noble dwarf.\n\nYou have bested the dungeon and your traitorous kin.\nReturn now, live in luxury, and enjoy your honor!";
                }
                else
                {
                    endGameSalute.text = "Feeble dwarf, you have been dishonored.\n\nYou will return a broken dwarf, or stay to haunt these halls forever.\nSuch is the fate of a weak dwarf.";
                }

                endGameHud.gameObject.SetActive(true);
                menuButton.Select();
            }
            else
            {
                //boss did not die, everyone else did, simply keep the base text and present the menu
                endGameHud.gameObject.SetActive(true);
            }
        }
    }

    public void OnDisable()
    {
        if (isLocalPlayer)
        {
            if (achievements) achievements.diedBy = 0;
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


    /// <summary>
    /// Notifies the manager that a local game should be played
    /// Allows local players to poll join in the starting room
    /// </summary>
    public void NotifyLocalGame()
    {
        awaitingLocalJoin = true;
    }

    /// <summary>
    /// Spawns a local player and adds its information to the player map
    /// NOTE: we link to the negative count of additional local players since they cannot drop out and the count is negated.
    ///      Although local multiplayers will not appear in a networked game, due to being monobehaviours, we can ensure that
    ///      these negative values are not going to be chosen by the host (0) and additional players.
    /// </summary>
    void SpawnLocalPlayer()
    {
        //get new ID
        additionalLocalPlayers++;
        //instantiate and link
        GameObject localPlayer = Instantiate(localMultiplayerPrefab, NetManScript.Instance.p1startPt.position, Quaternion.identity);
        localPlayer.GetComponent<LocalPlayerIdentity>().ID = -additionalLocalPlayers;
        NetManScript.Instance.LinkObjectToID(localPlayer, -additionalLocalPlayers);

        updateCamera();
    }

    [TargetRpc]
    void TargetSendScore(NetworkConnection conn, int score)
    {
        AcheivementScript.Instance.score = score;
    }

    [ClientRpc]
    void RpcSendTiming(float time)
    {
        if(time < 300)
        {
            AcheivementScript.Instance.fiveMinutes = true;
            if(time < 240)
            {
                AcheivementScript.Instance.fourMinutes = true;
                if(time < 180)
                {
                    AcheivementScript.Instance.threeMinutes = true;
                    if(time < 120)
                    {
                        AcheivementScript.Instance.twoMinutes = true;
                    }
                }
            }
        }
    }
}
