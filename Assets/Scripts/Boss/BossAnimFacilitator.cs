using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimFacilitator : MonoBehaviour {

    Animator anim; //animator controller
    Collider2D col; //collider
    BossAction actions; //actions
    [SerializeField]
    float timeToNextAttack; //time until the next attack is triggered
    bool IDLE; //whether the boss is idle and can get a new attack

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        actions = GetComponent<BossAction>();
        IDLE = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (IDLE)
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

        anim.SetTrigger(trigger);
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
}
