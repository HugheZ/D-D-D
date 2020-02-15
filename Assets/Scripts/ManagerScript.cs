using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    public List<GameObject> doors;
    List<Vector2> spawnPoints;
    List<Vector3> cameraPts;
    public Vector2 reSpawnPt;
    public GameObject player;
    public Camera camera;
    public AudioSource endPlayer;
    //public AudioClip endAudio;
    public AudioClip click;

    int curRoom = 0;
    private int roomsCleared;

    public Text score;  //score text
    public Text finalScore; //final score box
    public Image healthImage;   //health image to use as slider
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

        spawnPoints = new List<Vector2>();
        cameraPts = new List<Vector3>();

        spawnPoints.Add(new Vector2(13,2));
        spawnPoints.Add(new Vector2(28, 2));
        spawnPoints.Add(new Vector2(-1, -10.5f));
        spawnPoints.Add(new Vector2(24, -15));
        spawnPoints.Add(new Vector2(0, 2));

        if (curRoom - 1 < 0)
            reSpawnPt = spawnPoints[spawnPoints.Count - 1];
        else
            reSpawnPt = spawnPoints[curRoom - 1];


        cameraPts.Add(new Vector3(13.5f, 6.5f, -12));
        cameraPts.Add(new Vector3(28f, 7, -12.5f));
        cameraPts.Add(new Vector3(4, -7, -12f));
        cameraPts.Add(new Vector3(24, -8, -15f));
        cameraPts.Add(new Vector3(0, 6.5f, -12));


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
        player.transform.position = spawnPoints[curRoom];
        camera.transform.position = cameraPts[curRoom];
        if (curRoom == spawnPoints.Count - 1)
            curRoom = 0;
        else
            curRoom++;
        doors[curRoom].GetComponent<DoorScript>().CreateDoor();
        roomsCleared++;
        if (curRoom - 1 < 0)
            reSpawnPt = spawnPoints[spawnPoints.Count - 1];
        else
            reSpawnPt = spawnPoints[curRoom - 1];
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
    /// Calculates the new health for the player
    /// </summary>
    /// <param name="newHealth">The new health of the player</param>
    public void PlayerHit(int newHealth)
    {
        //set UI
        healthImage.fillAmount = newHealth / 100f;
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
