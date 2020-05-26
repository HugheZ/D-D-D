using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BossAnimFacilitator : NetworkBehaviour {

    Animator anim; //animator controller
    Collider2D col; //collider
    BossAction actions; //actions
    [SerializeField]
    float timeToNextAttack; //time until the next attack is triggered
    bool IDLE; //whether the boss is idle and can get a new attack
    SpriteMask mask; //the bubble sprite mask
    SpriteRenderer rend; //the renderere that has our sprite

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        actions = GetComponent<BossAction>();
        mask = GetComponent<SpriteMask>();
        rend = GetComponent<SpriteRenderer>();
        IDLE = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (IDLE && isServer)
        {
            IDLE = false;
            Invoke("GenerateNextAttack", timeToNextAttack);
        }
	}

    /// <summary>
    /// Generates the next attack in the sequence
    /// 0-3: shoot, 4-7: move, 8-9: jump
    /// </summary>
    void GenerateNextAttack()
    {
        int choice = Random.Range(0, 10);

        string trigger = "";

        if (choice <= 3)
        {
            trigger = "shoot";
        }
        else if (choice <= 7)
        {
            trigger = "move";
        }
        else if (choice <= 9)
        {
            trigger = "jump";
        }
        else trigger = "undf";

        RpcTriggerAnimation(trigger);
    }

    /// <summary>
    /// Triggers an animation over the client
    /// </summary>
    /// <param name="aTrig"></param>
    [ClientRpc]
    void RpcTriggerAnimation(string aTrig)
    {
        if(aTrig != "") anim.SetTrigger(aTrig);
    }

    public void SetIDLE()
    {
        IDLE = true;
    }

    public void SetAttack()
    {
        anim.SetBool("attack", true);
    }

    public void UnsetAttack()
    {
        anim.SetBool("attack", false);
    }

    public void SetCharge()
    {
        anim.SetBool("face_charge", true);
    }

    public void UnsetCharge()
    {
        anim.SetBool("face_charge", false);
    }

    public void SetHurt()
    {
        anim.SetTrigger("face_hurt");
    }

    /// <summary>
    /// All logic associated with jumping the boss
    /// </summary>
    public void Jump()
    {
        SetAttack();
        col.enabled = false;
    }

    /// <summary>
    /// All logic associated with landing the boss
    /// </summary>
    public void Land()
    {
        UnsetAttack();
        col.enabled = true;
    }

    /// <summary>
    /// Sets the boss to death
    /// </summary>
    public void Die()
    {
        anim.SetBool("die", true);
    }

    /// <summary>
    /// Completely resets the animation state of the boss
    /// </summary>
    public void ResetState()
    {
        anim.SetBool("die", true);
        UnsetAttack();
        UnsetCharge();
        foreach (string trigger in new string[]{ "shoot", "jump", "move", "hurt", "face_hurt"})
        {
            anim.ResetTrigger(trigger);
        }
    }

    /// <summary>
    /// Updates the sprite mask of this boss
    /// </summary>
    public void UpdateMask()
    {
        mask.sprite = rend.sprite;
        mask.UpdateGIMaterials();
    }
}
