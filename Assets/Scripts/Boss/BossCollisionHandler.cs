using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BossCollisionHandler : NetworkBehaviour {

    [SerializeField]
    [SyncVar(hook ="UpdateUI")]
    float health; //the boss' health
    BossAnimFacilitator anim; //boss animator
    [SerializeField]
    float timeUntilDeactive; //time until deactivate after death

	// Use this for initialization
	void Start () {
        anim = GetComponent<BossAnimFacilitator>();
	}

    /// <summary>
    /// Handles collision logic
    /// 
    /// SERVER: handles collisions
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer)
        {
            //if weapon, decrease health
            if (collision.gameObject.CompareTag("Weapons"))
            {
                Slasher sl = collision.gameObject.GetComponentInParent<Slasher>();
                if (sl)
                {
                    //notify server
                    DamageBoss(sl.damagePerSwing);
                    //increment that player's score
                    NetworkConnection scoringPlayer = collision.gameObject.GetComponentInParent<NetworkIdentity>().connectionToClient;
                    BossManager.Instance.UpdateScore(scoringPlayer, sl.damagePerSwing);
                }
            }
        }
    }

    /// <summary>
    /// Damages the boss and sets healthbar
    /// </summary>
    /// <param name="damage">Damage to be dealt to the boss</param>
    void DamageBoss(float damage)
    {
        health -= damage;
        //update bar
        UpdateUI(health);
    }

    /// <summary>
    /// Updates the boss healthbar
    /// </summary>
    /// <param name="HP">new HP for the boss</param>
    void UpdateUI(float HP)
    {
        //if death
        if (HP <= 0)
        {
            BossManager.Instance.UpdateHealth(HP, timeUntilDeactive);
            //update anim
            anim.Die();
        }
        else if(HP >= 100)
        {
            BossManager.Instance.UpdateHealth(HP);
        }
        else
        {
            BossManager.Instance.UpdateHealth(HP);
            //update anim
            anim.SetHurt();
        }
    }

    /// <summary>
    /// Resets the boss' health
    /// </summary>
    public void ResetHealth()
    {
        health = 100;
        BossManager.Instance.UpdateHealth(health);
    }
}
