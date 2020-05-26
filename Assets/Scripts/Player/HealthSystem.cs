using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int health = 100;

    PlayerMovement mvr; //mover
    PlayerMovementLocal lmvr; //local mover
    Slasher slash;  //slash ability
    SlasherLocal lslash; //local slasher
    CollisionHandler colHand;   //colliio handler
    CollisionHandlerLocal lcolHand; //local handler
    Rigidbody2D rb; //player rigidbody
    Animator anim; //player animator
    public AudioSource PlayerAudio;
    public AudioClip death;
    public AudioClip damageSound;
    public Image healthImage;   //health image to use as slider
    AcheivementScript achievements;

    // Start is called before the first frame update
    void Start()
    {
        mvr = GetComponent<PlayerMovement>();
        if (!mvr) lmvr = GetComponent<PlayerMovementLocal>();
        slash = GetComponent<Slasher>();
        if (!slash) lslash = GetComponent<SlasherLocal>();
        colHand = GetComponent<CollisionHandler>();
        if (!colHand) lcolHand = GetComponent<CollisionHandlerLocal>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        achievements = AcheivementScript.Instance;
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
        if (health <= 0)
        {
            KillPlayer();
        }
        else
        {
            PlayerAudio.PlayOneShot(damageSound);
        }

        //inform manager
        PlayerHit(newHealth);
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
    /// Kills the player
    /// </summary>
    public void KillPlayer()
    {
        PlayerAudio.PlayOneShot(death);
        //disable movement, slashing, and collision handling
        if (mvr) mvr.dead = true;
        else lmvr.dead = true;
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (slash) slash.enabled = false;
        else lslash.enabled = false;
        if (colHand) colHand.dead = true;
        else lcolHand.dead = true;


        //disable all triggers/hitboxes in system
        foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>())
        {
            if (col.gameObject.CompareTag("GHOST")) col.enabled = true;
            else col.enabled = false;
        }
        GetComponent<BoxCollider2D>().enabled = false;

        //play animation of death
        anim.SetBool("dead", true);
        anim.SetTrigger("die");

        //All disabled, inform manager
        if(ManagerScript.Instance) ManagerScript.Instance.PlayerDied();
        if (MultiplayerRunManager.Instance) MultiplayerRunManager.Instance.PlayerDied();
    }
}
