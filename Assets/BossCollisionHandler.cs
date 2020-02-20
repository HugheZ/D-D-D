using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollisionHandler : MonoBehaviour {

    [SerializeField]
    float health; //the boss' health
    BossAnimFacilitator anim; //boss animator

	// Use this for initialization
	void Start () {
        anim = GetComponent<BossAnimFacilitator>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Hit");
        //if weapon, decrease health
        if (collision.gameObject.CompareTag("Weapons"))
        {
            Slasher sl = collision.gameObject.GetComponentInParent<Slasher>();
            if (sl)
            {
                //update anim
                anim.SetHurt();
                //damage
                DamageBoss(sl.damagePerSwing);
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
        //TODO
    }
}
