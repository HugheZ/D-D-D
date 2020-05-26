using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GoldRoomManager : NetworkBehaviour {

    //singleton stuff
    private static GoldRoomManager _instance;

    public static GoldRoomManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    bool outOfGold = false;

    [SerializeField]
    GoldZone zone; //the gold zone object

    [SerializeField]
    Canvas goldHUD; //the hud for the gold status

    [SerializeField]
    Image goldImage; //image showing gold left

    [SerializeField]
    GameObject[] goldPiles; //gold piles to enable/disable based on gold left
    //NOTE: EXPECTS SIZE 4

    [SerializeField]
    ParticleSystem source; //particle source

    [SerializeField]
    Transform dest; //particle destination

    [SerializeField]
    AudioSource music; //music to play on enable

    List<GameObject> playersPresent; //number of players present in the gold zone
    float counter; //the current time counter for player in zone

    const int MAX_GOLD = 100; //max gold in the room
    const int SECONDS_PER_TICK = 2; //how many seconds to wait before updating 1 gold tic
    const int GOLD_PER_TICK = 2; //how much gold is taken per tick

    [SyncVar(hook = "UpdateGoldUI")]
    int goldLeft = 100; //current gold left

	// Use this for initialization
	void Start () {
        playersPresent = new List<GameObject>();
	}

    /// <summary>
    /// Enables the gold room for play
    /// </summary>
    public void EnableGoldRoom()
    {
        //set gold room active
        this.enabled = true;
        //set children active
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        //enable music
        music.Play();
    }
	
	// Update is called once per frame
	void Update () {
        //if exactly one player in the zone, count and show effect
        if (playersPresent.Count == 1)
        {
            //only count if server
            if (isServer)
            {
                counter += Time.deltaTime;

                //if we elapse a tick, add score
                if (counter > SECONDS_PER_TICK)
                {
                    //tick the score
                    TickScore(0);

                    //if no gold left, call endgame
                    if(goldLeft <= 0 && !outOfGold)
                    {
                        outOfGold = true;
                        HandleOutOfGold();
                    }
                }
            }

            //for all clients, move the gold around if we need to show the effect
            //if (!source.isPlaying) source.Play();
            //update transform
            dest.position = playersPresent[0].transform.position;
        }
    }

    /// <summary>
    /// Ticks the score of the player in the list, updates score UIs
    /// </summary>
    /// <param name="player">The index of the player to increment</param>
    void TickScore(int player)
    {
        //reset counter
        counter = 0;
        //change gold left (triggers syncvar to clients to update ui)
        goldLeft -= GOLD_PER_TICK;
        //call update UI function locally
        UpdateGoldUI(goldLeft);
        //get ID, either networked or local
        NetworkIdentity NID = playersPresent[player].GetComponent<NetworkIdentity>();
        int ID = NID ? NID.connectionToClient.connectionId : playersPresent[player].GetComponent<LocalPlayerIdentity>().ID;
        //give score
        MultiplayerRunManager.Instance.AwardPointsByID(ID, GOLD_PER_TICK);
    }

    /// <summary>
    /// Updates the gold UI
    /// If less than threholds, update gold on floor as well
    /// </summary>
    /// <param name="left">The gold left in the game</param>
    void UpdateGoldUI(int left)
    {
        //update var
        if(!isServer) goldLeft = left;

        //percentage left
        float percentage = goldLeft / ((float)MAX_GOLD);
        //update slider
        goldImage.fillAmount = percentage;

        //depending on gold left percentage, hide gold
        if(percentage <= 0f)
        {//no gold left
            goldPiles[3].SetActive(false);
        }
        else if(percentage <= .25f)
        {//25%ish left
            goldPiles[2].SetActive(false);
        }
        else if(percentage <= .5f)
        {//50%ish left
            goldPiles[1].SetActive(false);
        }
        else if(percentage <= .75f)
        {//75%ish left
            goldPiles[0].SetActive(false);
        }
        //a lot left, don't disable anything
    }

    
    /// <summary>
    /// Notifies the manager of a player entering/exiting the zone
    /// </summary>
    /// <param name="entered">True if a player entered the zone, false if they exited it</param>
    /// <param name="player">The player object that entered/exited the zone</param>
    public void NotifyOfPlayerZoneInteraction(bool entered, GameObject player)
    {
        //always update local list
        if (entered)
        {
            AddToList(player);
        }
        else
        {
            RemoveFromList(playersPresent.IndexOf(player));
        }

        //only listen if we are the server
        if (isServer)
        {
            //in both cases, set counter back to 0 if 1 player is left in the ring
            if (playersPresent.Count == 1) counter = 0;
        }
    }

    /// <summary>
    /// Pingable method that tells this manager to check if any players are dead in its inventory
    /// </summary>
    public void CheckForDeadPlayers()
    {
        //call rpc for players
        RpcUpdatePresentList();
        UpdatePresentList();
    }

    [ClientRpc]
    void RpcUpdatePresentList()
    {
        UpdatePresentList();
    }

    /// <summary>
    /// Updates the present list to remove anyone who is dead
    /// </summary>
    void UpdatePresentList()
    {
        //loop through players
        for (int i = 0; i < playersPresent.Count; i++)
        {
            //if dead, remove and set back iterator
            if (playersPresent[i].GetComponent<HealthSystem>().health <= 0)
            {
                RemoveFromList(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Removes a player from the present lsit and toggles the effect if not 1 player in list
    /// </summary>
    /// <param name="index">Index to remove from</param>
    void RemoveFromList(int index)
    {
        if (index < 0 || index > playersPresent.Count) throw new System.Exception("Removing player outside of lsit");
        playersPresent.RemoveAt(index);
        //stop effect if need be
        if(playersPresent.Count != 1)
        {
            source.Stop();
        }
        else
        {
            source.Play();
        }
    }

    /// <summary>
    /// Adds a player to the present lsit and toggles the effect if 1 player in list
    /// </summary>
    /// <param name="player">Player to add</param>
    void AddToList(GameObject player)
    {
        playersPresent.Add(player);
        //stop effect if need be
        if (playersPresent.Count == 1)
        {
            source.Play();
        }
        else
        {
            source.Stop();
        }
    }

    /// <summary>
    /// Handles all procedures required when we run out of gold
    /// </summary>
    void HandleOutOfGold()
    {
        //notify MRM we are done
        if (MultiplayerRunManager.Instance) MultiplayerRunManager.Instance.RoundOver();
    }
}
