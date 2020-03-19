using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BossManager : NetworkBehaviour {

    private static BossManager _instance;

    public static BossManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Canvas bossUI; //the UI for the boss
    public Image bossHealth; //the health slider for the boss
    public GameObject boss; //the boss in question
    public GameObject bossPrefab; //boss prefab
    public AudioSource music; //the boss music source

    public bool enableFromStart; //should the boss be enabled from start

	// Use this for initialization
	void Start () {
        ResetBoss(false);
        bossUI.gameObject.SetActive(false);
        if (enableFromStart) EnableBoss();
	}

    /// <summary>
    /// Resets the boss for the next round
    /// </summary>
    /// <param name="totalReset">Whether to reset all of the boss' parameters</param>
    void ResetBoss(bool totalReset)
    {
        if (!boss)
        {
            boss = Instantiate(bossPrefab, Vector2.zero, Quaternion.identity);
            boss.SetActive(false);
        }
        else
        {
            boss.SetActive(false);
        }

        if (totalReset)
        {
            boss.GetComponent<BossAction>().enabled = true;
            BossCollisionHandler col = boss.GetComponent<BossCollisionHandler>();
            col.enabled = true;
            col.ResetHealth();
            BossAnimFacilitator anim = boss.GetComponent<BossAnimFacilitator>();
            anim.ResetState();
        }

        //disable UI
        bossUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the boss client side and disables it
    /// </summary>
    /// <param name="totalReset">Whether to reset all of the boss' parameters</param>
    [ClientRpc]
    void RpcResetBoss(bool totalReset)
    {
        ResetBoss(totalReset);
    }

    /// <summary>
    /// Enables the boss encounter
    /// </summary>
    public void EnableBoss()
    {
        boss.SetActive(true);
        bossUI.gameObject.SetActive(true);
        music.Play();
    }

    /// <summary>
    /// Sets the Boss UI to take damage, handles end game
    /// </summary>
    /// <param name="health">New health percentage to set the UI to</param>
    /// <param name="timeUntilDeactivate">Time until the boss dactivates</param>
    public void UpdateHealth(float health, float timeUntilDeactivate = 0f)
    {
        bossHealth.fillAmount = health / 100f;
        if(health <= 0)
        {
            //trigger endgame
            //end music
            music.Stop();
            //reset for new game
            //disable collisions and actions
            boss.GetComponent<BossAction>().enabled = false;
            boss.GetComponent<BossCollisionHandler>().enabled = false;
            //deactivate all after some time
            Invoke("Deactivate", timeUntilDeactivate);

            // trigger main game manager to end the game
            MultiplayerRunManager.Instance.BossDefeated();
        }
    }

    /// <summary>
    /// Deactivates the boss
    /// </summary>
    void Deactivate()
    {
        ResetBoss(true);
    }

    /// <summary>
    /// Updates the correct player's score
    /// </summary>
    /// <param name="PID"></param>
    /// <param name="damage"></param>
    public void UpdateScore(NetworkConnection PID, float damage)
    {
        if (isServer)
        {
            //TODO
            MultiplayerRunManager.Instance.AwardPointsByID(PID,damage);
        }
    }
}
