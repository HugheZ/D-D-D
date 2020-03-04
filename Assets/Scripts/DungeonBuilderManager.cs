using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DungeonBuilderManager : MonoBehaviour {

    public GameObject cursor;
    public GameObject room;
    public GameObject fire;
    Color fireColor;
    public Button shuffleButton;
    public Text shuffleText;
    public List<GameObject> trapPool;
    public List<Transform> trapSpawnPts;
    System.Random rnd;
    int shufflesLeft;

    //stuff for Singleton
    private static DungeonBuilderManager _instance = null;
    public static DungeonBuilderManager Instance
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
        shufflesLeft = 3;
        shuffleText.text = "Shuffles Left: " + shufflesLeft;
        rnd = new System.Random();
        GetNewTraps();
        //TODO: GET PLAYER NUMBER + CHANGE FIRE COLOR
        fireColor = new Color(1, 0.458823529411764f, 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void GetNewTraps()
    {
        for(int i = 0; i < trapSpawnPts.Count; i++)
        {
            GameObject newTrap = trapPool[rnd.Next(0, trapPool.Count)];
            if (newTrap.GetComponent<ArrowTrapScript>())
                newTrap.GetComponent<ArrowTrapScript>().SetDisabled();
            if (newTrap.GetComponent<SawbladeScript>())
                newTrap.GetComponent<SawbladeScript>().FlipActive();
            Instantiate(newTrap, trapSpawnPts[i]);
        }
    }

    public Transform GetRoomTransform()
    {
        return room.transform;
    }

    public void ShuffleNewItems()
    {
        shufflesLeft--;
        shuffleText.text = "Shuffles Left: " + shufflesLeft;
        if (shufflesLeft == 0)
        {
            shuffleButton.enabled = false;
            shuffleButton.image.color = Color.red;
        }
        GetNewTraps();
    }
    public void FinishRoom()
    {
        GameObject fire1 = Instantiate(fire, new Vector2(-7.5f,-3.5f), Quaternion.identity, room.transform);
        fire1.GetComponent<SpriteRenderer>().color = fireColor;
        GameObject fire2 = Instantiate(fire, new Vector2(-2.5f, -3.5f), Quaternion.identity, room.transform);
        fire2.GetComponent<SpriteRenderer>().color = fireColor;
        GameObject fire3 = Instantiate(fire, new Vector2(-7.5f, 4.5f), Quaternion.identity, room.transform);
        fire3.GetComponent<SpriteRenderer>().color = fireColor;
        GameObject fire4 = Instantiate(fire, new Vector2(-2.5f, 4.5f), Quaternion.identity, room.transform);
        fire4.GetComponent<SpriteRenderer>().color = fireColor;
        //SceneManager.LoadScene("MainMenu");
    }
}
