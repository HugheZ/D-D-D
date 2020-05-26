using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CollisionHandler : NetworkBehaviour
{
    public bool canGetHurt = true;
    float timeSinceHurt;
    HealthSystem HP;
    ManagerScript manager;
    Rigidbody2D rb;
    SpriteRenderer srender;
    AcheivementScript acheivements;

    bool isLocal;

    //player knockback value
    public float knockback;

    //how long knockback is in effect
    public float knockbackStunTime;

    //audio source for playing door opening
    public AudioSource Door;

    //boolean for controlling death collisions
    public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        HP = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        manager = ManagerScript.Instance;
        srender = GetComponent<SpriteRenderer>();
        acheivements = AcheivementScript.Instance;
        if(MultiplayerRunManager.Instance != null)
        {
            isLocal = MultiplayerRunManager.Instance.isLocalMultiplayerGame;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //If you are in invincibility frames, make sure not enough time has passed to get out of them
            if (!canGetHurt)
            {
                if (Time.time - timeSinceHurt > 2)
                {
                    canGetHurt = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLocalPlayer && !dead)
        {
            if (collision.gameObject.GetComponent<SpikeScript>() != null && canGetHurt)
            {
                //ground is spikes
                HurtPlayer(8, 5);
            }
            else if (collision.gameObject.tag == "Pit")
            {
                //hit a pit, fall
                FallInPit();
            }else if (collision.gameObject.tag == "SavePoint")
            {
                MultiplayerRunManager.Instance.player1RespawnPoint = collision.gameObject.transform.position;
                
            }
            //check to see if hit a person with our axe
            Slasher slash = collision.gameObject.GetComponentInParent<Slasher>();
            SlasherLocal lslash = collision.gameObject.GetComponentInParent<SlasherLocal>();
            if ((slash || lslash) && collision.gameObject.transform.parent != transform && canGetHurt && !collision.gameObject.CompareTag("Hurtbox"))
            {
                //TODO: BOTH trigger and RigidBody2D receive this message. Check here to see if the trigger involved is ours or not (if so, this is our hit, do nothing, else, we were hit by someone else, get hurt)
                Vector2 dir = (transform.position - collision.gameObject.transform.parent.position).normalized;
                float angle = Vector2.SignedAngle(Vector2.right, dir);
                print("DIRECTION: " + dir + "ANGLE: " + angle);
                HurtPlayer(0, -1, true, angle);
            }
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (isLocalPlayer)
    //    {
    //        if (collision.gameObject.GetComponent<SpikeScript>() != null && canGetHurt)
    //        {
    //            HurtPlayer(8, 5);
    //        }
    //    }
    //}

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
        if (manager) gameObject.transform.position = manager.reSpawnPt;
        else gameObject.transform.position = MultiplayerRunManager.Instance.GetRespawnPoint(gameObject, 1);
    }

    /// <summary>
    /// Logic for player landing back on the ground
    /// </summary>
    public void Land()
    {
        //reinstate movement and collision
        ToggleInteractivity(true);
        //hurt player
        HurtPlayer(20, 1);
    }

    /// <summary>
    /// Toggles collision and movement logic for pits
    /// </summary>
    public void ToggleInteractivity(bool toggle)
    {
        //toggle movement
        PlayerMovement m = GetComponent<PlayerMovement>();
        if (toggle) m.SetMove();
        else m.DisableMove();
        //toggle all triggers/hitboxes in system, toggle weapons off but never on
        foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>())
        {
            if (!col.gameObject.CompareTag("Weapons") || !toggle) col.enabled = toggle;
        }
        GetComponent<BoxCollider2D>().enabled = toggle;
        //toggle velocity
        if (!toggle) rb.constraints = RigidbodyConstraints2D.FreezeAll;
        else rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Slasher sl = GetComponent<Slasher>();
        if (sl.isSwinging) sl.EndSlash();
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
        if (isLocalPlayer && !dead)
        {
            if (collision.gameObject.GetComponent<SawbladeScript>() != null)
            {
                if (canGetHurt)
                {
                    //we hit a sawblade, take damage
                    HurtPlayer(16, 3);
                }
            }
            else if (collision.gameObject.GetComponent<ArrowScript>() != null)
            {
                //an arrow hit us
                if (canGetHurt)
                {
                    HurtPlayer(12, 2);
                }
            }
            else if (collision.gameObject.GetComponent<Bubble>() != null)
            {
                if (canGetHurt)
                {
                    HurtPlayer(5, 6);
                }
            }
            else if (collision.gameObject.GetComponent<Fireball>() != null)
            {
                if (canGetHurt)
                {
                    HurtPlayer(10, 4);
                }
            }
            else if (collision.gameObject.GetComponent<BossAction>() != null)
            {
                if (canGetHurt)
                {
                    HurtPlayer(16, 7);
                }
            }
            else if (collision.gameObject.tag == "Door")
            {
                Door.PlayOneShot(Door.clip);
            }
        }
    }

    /// <summary>
    /// Pass function that informs the death handler of hurt
    /// </summary>
    /// <param name="damage">dmage taken</param>
    /// <param name="hitBy">What hit the player? Used for achievements</param>
    /// <param name="applyKnockback">Whether to apply knockback</param>
    /// <param name="launchAngle">Direction to knockback</param>
    private void HurtPlayer(int damage, int hitBy, bool applyKnockback = false, float launchAngle = 0)
    {
        if(HP.health - damage <= 0)
        {
            if (AcheivementScript.Instance)
            {
                AcheivementScript.Instance.diedBy = hitBy;
            }
        }
       

        if (!isServer) CmdHurtPlayer(damage, applyKnockback, launchAngle);
        else
        {
            //say he got hurt and give damage
            RpcHurtPlayer(damage);
            //apply knockback if needed
            if (applyKnockback)
            {
                NetworkConnection con = NetworkServer.objects[netId].connectionToClient;
                TargetLaunch(con, launchAngle);
            }
        }
    }


    private IEnumerator flickerSprite()
    {
        int flickerCounter = 0;
        float flickerTime = Time.time;
        while (Time.time - flickerTime < 2)
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


    /// <summary>
    /// Tells server to relay hurt player command
    /// </summary>
    [Command]
    void CmdHurtPlayer(int damage, bool applyKnockback, float launchAngle)
    {
        RpcHurtPlayer(damage);
        //apply knockback if needed
        if (applyKnockback)
        {
            NetworkConnection con = NetworkServer.objects[netId].connectionToClient;
            TargetLaunch(con, launchAngle);
            RpcResetAnimationControl();
        }
    }

    /// <summary>
    /// Hurts the player for x damage
    /// </summary>
    /// <param name="damage">Damage to hurt the player</param>
    /// <param name="hitBy">What hit the player? Used for achievements</param>
    /// <param name="applyKnockback">Whether to apply knockback</param>
    [ClientRpc]
    void RpcHurtPlayer(int damage)
    {
        if (!dead) { 
            if (canGetHurt || !isLocalPlayer)
            {
                if (damage > 0) getInvincibilityFrames();
                else StartCoroutine("flickerSprite");
                HP.HurtPlayer(damage);
            }
        }
    }

    /// <summary>
    /// Tells this specific player to apply knockback
    /// Other players will get the update via transform sync
    /// </summary>
    /// <param name="con">Connection to target</param>
    /// <param name="angle">Angle to launch</param>
    [TargetRpc]
    void TargetLaunch(NetworkConnection con, float angle)
    {
        //if player isn't dead, apply knockback
        if (HP.health > 0)
        {
            //disable movement
            PlayerMovement pm = GetComponent<PlayerMovement>();
            pm.DisableMove();
            //apply knockback
            Vector2 force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * knockback;
            print(force);
            rb.AddForce(force, ForceMode2D.Impulse);

            Invoke("ReEnableMovement", knockbackStunTime);
        }
    }

    /// <summary>
    /// Reenables mvoement after knockback if still alive
    /// </summary>
    void ReEnableMovement()
    {
        if(HP.health > 0)
        {
            PlayerMovement pm = GetComponent<PlayerMovement>();
            pm.SetMove();
            pm.ResetAnimationState();
        }
    }

    /// <summary>
    /// Resets the animation controller on all clients' player, prevents animation lock
    /// </summary>
    [ClientRpc]
    void RpcResetAnimationControl()
    {
        GetComponent<PlayerMovement>().ResetAnimationState();
    }
}
