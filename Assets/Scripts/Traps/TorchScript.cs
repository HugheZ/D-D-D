using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchScript : MonoBehaviour {

    [SerializeField]
    Fireball fb; //the fireball to shoot
    [SerializeField]
    float predictionDistanceMult; //distance multiplier to predict movement
    [SerializeField]
    float fireballSpeed; //speed to launch fireballs
    [SerializeField]
    Vector3 spawnOrigin; //origin of spawn, relative to center of game object
    [SerializeField]
    float spawnDist; //distance from origin to spawn

    //The list of players to track
    private List<Rigidbody2D> playerList;

    AudioSource sound;

	// Use this for initialization
	void Start () {
        playerList = new List<Rigidbody2D>();
        sound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Shoots a fireball at a random player in range, calculates angle by predicting distance
    /// </summary>
    public void Shoot()
    {
        // return if no players to shoot
        if (playerList.Count == 0) return;

        Rigidbody2D toShoot = playerList[Random.Range(0, playerList.Count)];

        //if no one to shoot, return
        if (!toShoot) return;

        //predict new position based on velocity and prediction distance
        Vector2 target = toShoot.position + toShoot.velocity.normalized * (Vector2.Distance(toShoot.position, transform.position) * predictionDistanceMult);
        Vector3 direction = (target - (Vector2) transform.position).normalized;
        float angle = Vector2.Angle(Vector2.up, direction);
        Vector3 spawnPos = transform.position + spawnOrigin + direction * spawnDist;
        GameObject instance;

        //get from pooler, fallback to instantiate
        if (ObjectPooler.FireballSharedInstance)
        {
            instance = ObjectPooler.FireballSharedInstance.GetPooledObject();
            //if no fireball to spawn, return
            if (!instance) return;

            //set position
            instance.transform.position = spawnPos;
            //set active
            instance.SetActive(true);
        }
        else
        {
            instance = Instantiate(fb, spawnPos, Quaternion.identity).gameObject;
        }
        //play audio
        sound.Play();

        //shoot it
        instance.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed;
        
    }

    //Ontriggerenter, add this player to the list
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerList.Add(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    //Ontriggerexit, remove this player
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerList.Remove(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }
}
