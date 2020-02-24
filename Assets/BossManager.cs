using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour {

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

    public bool enableFromStart; //should the boss be enabled from start

	// Use this for initialization
	void Start () {
        if (!boss)
        {
            boss = Instantiate(bossPrefab, Vector2.zero, Quaternion.identity);
            boss.SetActive(enableFromStart);
        }
        else
        {
            boss.SetActive(enableFromStart);
        }

        bossUI.gameObject.SetActive(enableFromStart);
	}

    /// <summary>
    /// Enables the boss encounter
    /// </summary>
    public void EnableBoss()
    {
        boss.SetActive(true);
        bossUI.gameObject.SetActive(true);
        //TODO: enable boss music
    }

    /// <summary>
    /// Sets the Boss UI to take damage, handles end game
    /// </summary>
    /// <param name="health">New health percentage to set the UI to</param>
    public void UpdateHealth(float health)
    {
        bossHealth.fillAmount = health / 100f;
        if(health == 0)
        {
            //TODO: trigger endgame
        }
    }
}
