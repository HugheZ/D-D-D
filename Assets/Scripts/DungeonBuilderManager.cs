using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilderManager : MonoBehaviour {

    public GameObject cursor;
    public GameObject room;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Transform GetRoomTransform()
    {
        return room.transform;
    }
}
