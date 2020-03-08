using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerRunManager : MonoBehaviour {

    public GameObject p1room;
    public GameObject p2room;
    public GameObject p3room;
    public GameObject p4room;
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
    

	// Use this for initialization
	void Start () {
        if (p1room == null)
        {
            p1room = GameObject.FindGameObjectWithTag("P1Room");
            ArrowTrapScript[] arrowTraps = (ArrowTrapScript[])FindObjectsOfType(typeof(ArrowTrapScript));
            foreach(ArrowTrapScript arrowTrap in arrowTraps)
                arrowTrap.SetEnabled();
            SawbladeScript[] sawblades = (SawbladeScript[])FindObjectsOfType(typeof(SawbladeScript));
            foreach (SawbladeScript sawblade in sawblades)
                sawblade.FlipActive();
            p1room.SetActive(false);
            p1room.transform.position = new Vector2(0, 0);
        }
        /*if (p2room == null)
        {
            p2room = GameObject.FindGameObjectWithTag("P2Room");
            p2room.SetActive(false);
        }
        if (p3room == null)
        {
            p3room = GameObject.FindGameObjectWithTag("P3Room");
            p3room.SetActive(false);
        }
        if (p4room == null)
        {
            p4room = GameObject.FindGameObjectWithTag("P4Room");
            p4room.SetActive(false);
        }*/

        //start in rooms
        GameObject p1r1 = Instantiate(p1room, p1Space);
        GameObject p2r1 = Instantiate(p1room, p2Space);
        GameObject p3r1 = Instantiate(p1room, p3Space);
        GameObject p4r1 = Instantiate(p1room, p4Space);
        
        p1r1.SetActive(true);
        p2r1.SetActive(true);
        p3r1.SetActive(true);
        p4r1.SetActive(true);

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
