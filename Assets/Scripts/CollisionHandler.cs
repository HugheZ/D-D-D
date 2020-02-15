using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public bool canGetHurt = true;
    float timeSinceHurt;
    HealthSystem HP;
    ManagerScript manager;
    Rigidbody2D rb;
	SpriteRenderer srender;

    public AudioSource Door;

    // Start is called before the first frame update
    void Start()
    {
        HP = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        manager = ManagerScript.Instance;
        srender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //If you are in invincibility frames, make sure not enough time has passed to get out of them
        if (!canGetHurt)
        {
            if(Time.time - timeSinceHurt > 2)
            {
                canGetHurt = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpikeScript>() != null) {
            //ground is spikes
            if (canGetHurt)
            {
                HurtPlayer(8);
                getInvincibilityFrames();
            }
        }
        else if(collision.gameObject.tag == "Pit")
        {
            //hit a pit, fall
            FallInPit();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpikeScript>() != null && canGetHurt)
        {
            HurtPlayer(8);
            getInvincibilityFrames();
        }
    }

    /// <summary>
    /// Handles logic for falling into a pit, triggers animation
    /// </summary>
    void FallInPit()
    {
        //disable interactivity to prevent double hits or anim disrupt
        ToggleInteractivity(false);
        //tell animator
        GetComponent<Animator>().SetTrigger("fall");
    }

    /// <summary>
    /// Resets player position to spawn point
    /// </summary>
    public void ResetToSpawn()
    {
        gameObject.transform.position = manager.reSpawnPt;
    }

    /// <summary>
    /// Logic for player landing back on the ground
    /// </summary>
    public void Land()
    {
        //reinstate movement and collision
        ToggleInteractivity(true);
        //hurt player
        HurtPlayer(20);
        getInvincibilityFrames();
    }

    /// <summary>
    /// Toggles collision and movement logic for pits
    /// </summary>
    void ToggleInteractivity(bool toggle)
    {
        //toggle movement
        GetComponent<PlayerMovement>().enabled = toggle;
        //toggle all triggers/hitboxes in system
        foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>()) col.enabled = toggle;
        GetComponent<BoxCollider2D>().enabled = toggle;
        //toggle velocity
        if (!toggle) rb.constraints = RigidbodyConstraints2D.FreezeAll;
        else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void getInvincibilityFrames()
    {
        //Make sure player can't get hurt for a certain amount of seconds
        canGetHurt = false;
        timeSinceHurt = Time.time;
        //Flicker player sprite
        StartCoroutine("flickerSprite");
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<SawbladeScript>() != null)
        {
            if (canGetHurt)
            {
                //we hit a sawblade, take damage
                HurtPlayer(16);

                //launch player backward
                int magnitude = -25000;
                Vector2 force = transform.position - collision.gameObject.transform.position;
                force.Normalize();
                rb.velocity = new Vector2(force.x, force.y) * magnitude;
                print(rb.velocity);

                //get I-frames
                getInvincibilityFrames();
            }
        }
        else if (collision.gameObject.GetComponent<ArrowScript>() != null)
        {
            //an arrow hit us
            if (canGetHurt)
            {
                HurtPlayer(12);
                getInvincibilityFrames();
            }
        }
        else if (collision.gameObject.tag == "Door")
        {
            Door.PlayOneShot(Door.clip);
        }
        print(collision.gameObject.tag);
        
    }

    /// <summary>
    /// Pass function that informs the death handler of hurt
    /// </summary>
    /// <param name="damage">dmage taken</param>
    private void HurtPlayer(int damage)
    {
        HP.HurtPlayer(damage);
    }

    
    
    private IEnumerator flickerSprite()
    {
        int flickerCounter = 0;
        float flickerTime = Time.time;
        while (Time.time - timeSinceHurt < 2)
        {
            flickerCounter++;
            if (flickerCounter % 2 == 0)
            {
                srender.color = Color.white;
            }
            else
            {
                srender.color = Color.red;
            }
            yield return new WaitForSeconds(.05f);
        }
        srender.color = Color.white;
        
    }
}
