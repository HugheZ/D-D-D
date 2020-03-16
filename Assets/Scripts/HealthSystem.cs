using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 100;

    PlayerMovement mvr; //mover
    Slasher slash;  //slash ability
    CollisionHandler colHand;   //colliio handler
    Rigidbody2D rb; //player rigidbody
    Animator anim; //player animator
    public AudioSource PlayerAudio;
    public AudioClip death;
    public AudioClip damageSound;

    // Start is called before the first frame update
    void Start()
    {
        mvr = GetComponent<PlayerMovement>();
        slash = GetComponent<Slasher>();
        colHand = GetComponent<CollisionHandler>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Hurts the player and checks for death
    /// </summary>
    /// <param name="damage">Damage to be dealt</param>
    public void HurtPlayer(int damage)
    {
        //decrement health
        health -= damage;

        UpdateGame(health);
    }

    /// <summary>
    /// Updates UI and manager of new health
    /// </summary>
    /// <param name="newHealth"></param>
    void UpdateGame(int newHealth)
    {
        //die if dead
        if (health <= 0) KillPlayer();
        else
        {
            PlayerAudio.PlayOneShot(damageSound);
        }

        //inform manager
        if(ManagerScript.Instance) ManagerScript.Instance.PlayerHit(newHealth);
    }

    /// <summary>
    /// Kills the player
    /// </summary>
    public void KillPlayer()
    {
        PlayerAudio.PlayOneShot(death);
        //disable movement, slashing, and collision handling
        mvr.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        slash.enabled = false;
        colHand.enabled = false;


        //disable all triggers/hitboxes in system
        foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>()) col.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        //play animation of death
        anim.SetBool("dead", true);
        anim.SetTrigger("die");

        //All disabled, inform manager
        ManagerScript.Instance.PlayerDied();
    }
}
