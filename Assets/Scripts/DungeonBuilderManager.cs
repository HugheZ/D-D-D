using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonBuilderManager : MonoBehaviour {

    public GameObject cursor;
    public GameObject room;
    public Button shuffleButton;
    public Text shuffleText;
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
    }
	
	// Update is called once per frame
	void Update () {
		
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
    }
    public void FinishRoom()
    {

    }
}
