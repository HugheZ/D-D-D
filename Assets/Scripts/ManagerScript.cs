using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    List<Vector3> cameraPts;
    public Vector2 reSpawnPt;
    public GameObject player;
    public List<GameObject> rooms;
    GameObject currentRoom;
    System.Random rand;
    public Camera camera;
    public AudioSource endPlayer;
    //public AudioClip endAudio;
    public AudioClip click;

    int curRoom = 0;
    private int roomsCleared;

    public Text score;  //score text
    public Text finalScore; //final score box
    private int totalScore; //the total score of the player
    public GameObject gameOverUI; //the ui to display when the game is over
    public float timeToShowEnd; //time until the end screen is shown after death

    //stuff for Singleton
    private static ManagerScript _instance = null;
    public static ManagerScript Instance
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
    // Start is called before the first frame update
    void Start()
    {
        roomsCleared = 0;

        cameraPts = new List<Vector3>();

        cameraPts.Add(new Vector3(13.5f, 6.5f, -12));
        cameraPts.Add(new Vector3(28f, 7, -12.5f));
        cameraPts.Add(new Vector3(4, -7, -12f));
        cameraPts.Add(new Vector3(24, -8, -15f));
        cameraPts.Add(new Vector3(0, 6.5f, -12));

        //start player in Room1
        currentRoom = Instantiate(rooms[0]);
        currentRoom.transform.GetChild(3).gameObject.SetActive(true);
        print("Spawning in Room1A");
        rand = new System.Random();
        reSpawnPt = new Vector2(0, 1);


        //UI stuff
        totalScore = 0;
        SetScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Goes to the next room
    /// </summary>
    public void NextRoom()
    {
        player.GetComponent<CollisionHandler>().ToggleInteractivity(false);
        Destroy(currentRoom);
        int traps = rand.Next(3);
        int newRoom = curRoom;
        while (newRoom == curRoom)
            newRoom = rand.Next(0, rooms.Count);
        curRoom = newRoom;
        player.transform.position = reSpawnPt;
        currentRoom = Instantiate(rooms[curRoom], new Vector3(0,0,0), Quaternion.identity);
        if(traps == 0)
        {
            currentRoom.transform.GetChild(3).gameObject.SetActive(true);
            print("A Room");
        }
        else if(traps == 1)
        {
            currentRoom.transform.GetChild(4).gameObject.SetActive(true);
            print("B Room");
        }
        else
        {
            currentRoom.transform.GetChild(5).gameObject.SetActive(true);
            print("C Room");
        }
        player.GetComponent<CollisionHandler>().ToggleInteractivity(true);
        roomsCleared++;
        AddScore(1);
    }

    /// <summary>
    /// Increases the player's score, sets UI
    /// </summary>
    /// <param name="points">The score to add to total</param>
    public void AddScore(int points)
    {
        totalScore += points;
        SetScoreUI();
    }

    /// <summary>
    /// Sets thes core to the UI
    /// </summary>
    private void SetScoreUI()
    {
        score.text = totalScore.ToString();
    }


    /// <summary>
    /// Triggers death cinematic
    /// </summary>
    public void PlayerDied()
    {
        //invoke show
        Invoke("ShowEnd", timeToShowEnd);
        //update final score
        finalScore.text = string.Format("Rooms Cleared:\n{0}", roomsCleared);
    }

    /// <summary>
    /// Invokable method to show the end screen
    /// </summary>
    private void ShowEnd()
    {
        //endPlayer.mute = true;
        //endPlayer.PlayOneShot(endAudio);
        gameOverUI.SetActive(true);
    }

    public void BackToMenu()
    {
        endPlayer.PlayOneShot(click);
        SceneManager.LoadScene("MainMenu");
    }
}
