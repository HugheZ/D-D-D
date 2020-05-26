using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AcheivementScript : MonoBehaviour {

    public GameObject achivementObject;
    ManagerScript manager;
    MultiplayerRunManager mrm;
    public int diedBy = 0;
    public bool flexed = false;
    public bool waved = false;
    public bool fiveMinutes, fourMinutes, threeMinutes, twoMinutes = false;
    public int score = 0;
    Color achieved = new Color(.4554f, 1, 0);
    public Canvas achievementCanvas, achievementUICanvas;
    public Button backButton;
    public Text achievementName;
    float UITime;

    public GameObject achievementPage1, achievementPage2, achievementPage3;

    public RawImage decade, quarter, half, century, impaled, shotDead, pitfall, burnedAlive, chopped, friendsKnife, betrayed, yoshi, prepared, burnedAtStake, betterThanYou, greetings, encapsulated, engulfed, dontNeedYou, treasureHorder, bossSlayer, pileOfGold, bossHelper, pocketfulOfGold, lolligagger, jogger, runner, sprinter;
    //stuff for Singleton
    private static AcheivementScript _instance = null;
    public static AcheivementScript Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }


    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(achivementObject);
        //Set all acheivements to not gotten if first time acheivements haven't been made
        if(!PlayerPrefs.HasKey("Achievements Made"))
        {
            //Already shown in game
            PlayerPrefs.SetInt("Achievements Made", 1);
            PlayerPrefs.SetInt("Decade of Rooms", 0);
            PlayerPrefs.SetInt("Century of Rooms", 0);
            PlayerPrefs.SetInt("Half-Century of Rooms", 0);
            PlayerPrefs.SetInt("Quarter-Century of Rooms", 0);
            PlayerPrefs.SetInt("Pitfall", 0);
            PlayerPrefs.SetInt("Impaled", 0);
            PlayerPrefs.SetInt("Shot Dead", 0);
            PlayerPrefs.SetInt("Burned Alive", 0);
            PlayerPrefs.SetInt("Chopped", 0);
            PlayerPrefs.SetInt("A Friends Knife", 0);
            PlayerPrefs.SetInt("Yoshi", 0);
            PlayerPrefs.SetInt("Betrayed", 0);
            PlayerPrefs.SetInt("Prepared", 0);
            PlayerPrefs.SetInt("Burned At The Stake", 0);
            PlayerPrefs.SetInt("I'm Better Than You", 0);
            PlayerPrefs.SetInt("Greetings", 0);
            PlayerPrefs.SetInt("Encapsulated", 0);
            PlayerPrefs.SetInt("Engulfed", 0);
            PlayerPrefs.SetInt("I Don't Need You", 0);
            PlayerPrefs.SetInt("Treasure Hoarder", 0);
            PlayerPrefs.SetInt("Nice Boss", 0);
            PlayerPrefs.SetInt("Nice Gold", 0);
            PlayerPrefs.SetInt("Boss Slayer", 0);
            PlayerPrefs.SetInt("Pile Of Gold", 0);
            PlayerPrefs.SetInt("Boss Helper", 0);
            PlayerPrefs.SetInt("Pocketful Of Gold", 0);
            PlayerPrefs.SetInt("Lolligagger", 0);
            PlayerPrefs.SetInt("Jogger", 0);
            PlayerPrefs.SetInt("Runner", 0);
            PlayerPrefs.SetInt("Sprinter", 0);
        }

        //Set all achievement canvas gameobjects to inactive
        achievementPage1.SetActive(false);
        achievementPage2.SetActive(false);
        achievementPage3.SetActive(false);

        //Set already achieved backgrounds to green
        if(PlayerPrefs.GetInt("Decade of Rooms") == 1)
        {
            decade.color = achieved;
        }
        if (PlayerPrefs.GetInt("Quarter-Century of Rooms") == 1)
        {
            quarter.color = achieved;
        }
        if (PlayerPrefs.GetInt("Half-Century") == 1)
        {
            half.color = achieved;
        }
        if (PlayerPrefs.GetInt("Century of Rooms") == 1)
        {
            century.color = achieved;
        }
        if (PlayerPrefs.GetInt("Impaled") == 1)
        {
            impaled.color = achieved;
        }
        if (PlayerPrefs.GetInt("Pitfall") == 1)
        {
            pitfall.color = achieved;
        }
        if (PlayerPrefs.GetInt("Shot Dead") == 1)
        {
            shotDead.color = achieved;
        }
        if (PlayerPrefs.GetInt("Burned Alive") == 1)
        {
            burnedAlive.color = achieved;
        }
        if (PlayerPrefs.GetInt("Chopped") == 1)
        {
            chopped.color = achieved;
        }
        if (PlayerPrefs.GetInt("A Friends Knife") == 1)
        {
            friendsKnife.color = achieved;
        }
        if (PlayerPrefs.GetInt("Yoshi") == 1)
        {
            yoshi.color = achieved;
        }
        if (PlayerPrefs.GetInt("Betrayed") == 1)
        {
            betrayed.color = achieved;
        }
        if (PlayerPrefs.GetInt("Prepared") == 1)
        {
            prepared.color = achieved;
        }
        if (PlayerPrefs.GetInt("Burned At The Stake") == 1)
        {
            burnedAtStake.color = achieved;
        }
        if (PlayerPrefs.GetInt("I'm Better Than You") == 1)
        {
            betterThanYou.color = achieved;
        }
        if (PlayerPrefs.GetInt("Greetings") == 1)
        {
            greetings.color = achieved;
        }
        if (PlayerPrefs.GetInt("Encapsulated") == 1)
        {
            encapsulated.color = achieved;
        }
        if (PlayerPrefs.GetInt("Engulfed") == 1)
        {
            engulfed.color = achieved;
        }
        if (PlayerPrefs.GetInt("I Don't Need You") == 1)
        {
            dontNeedYou.color = achieved;
        }
        if (PlayerPrefs.GetInt("Treasure Hoarder") == 1)
        {
            treasureHorder.color = achieved;
        }
        if (PlayerPrefs.GetInt("Boss Slayer") == 1)
        {
            bossSlayer.color = achieved;
        }
        if (PlayerPrefs.GetInt("Pile Of Gold") == 1)
        {
            pileOfGold.color = achieved;
        }
        if (PlayerPrefs.GetInt("Boss Helper") == 1)
        {
            bossHelper.color = achieved;
        }
        if (PlayerPrefs.GetInt("Pocketful Of Gold") == 1)
        {
            pocketfulOfGold.color = achieved;
        }
        if (PlayerPrefs.GetInt("Lolligagger") == 1)
        {
            lolligagger.color = achieved;
        }
        if (PlayerPrefs.GetInt("Jogger") == 1)
        {
            jogger.color = achieved;
        }
        if (PlayerPrefs.GetInt("Runner") == 1)
        {
            runner.color = achieved;
        }
        if (PlayerPrefs.GetInt("Sprinter") == 1)
        {
            sprinter.color = achieved;
        }
    }

    

    // Update is called once per frame
    void Update () {

        //Keep updating gamemode
        manager = ManagerScript.Instance;
        mrm = MultiplayerRunManager.Instance;

        //If achievementScene, show achievements...if not, don't
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "AchievementScene")
        {
            achievementCanvas.gameObject.SetActive(true);
            //backButton.Select();
        }
        else
        {
            achievementCanvas.gameObject.SetActive(false);
        }

        //Disable Achievement Get UI if been 5 seconds
        if(Time.time - UITime > 5)
        {
            achievementUICanvas.gameObject.SetActive(false);
        }

        //TODO: listen for misc achievements here
        if(flexed && PlayerPrefs.GetInt("I'm Better Than You") == 0)
        {
            betterThanYou.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "I'm Better Than You";
            UITime = Time.time;
            PlayerPrefs.SetInt("I'm Better Than You", 1);
        } 
        if (waved && PlayerPrefs.GetInt("Greetings") == 0)
        {
            greetings.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Greetings";
            UITime = Time.time;
            PlayerPrefs.SetInt("Greetings", 1);
        }
        if(fiveMinutes && PlayerPrefs.GetInt("Lolligagger") == 0)
        {
            lolligagger.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Lolligagger";
            UITime = Time.time;
            PlayerPrefs.SetInt("Lolligagger", 1);
        }
        if (fourMinutes && PlayerPrefs.GetInt("Jogger") == 0)
        {
            jogger.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Jogger";
            UITime = Time.time;
            PlayerPrefs.SetInt("Jogger", 1);
        }
        if (threeMinutes && PlayerPrefs.GetInt("Runner") == 0)
        {
            runner.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Runner";
            UITime = Time.time;
            PlayerPrefs.SetInt("Runner", 1);
        }
        if (twoMinutes && PlayerPrefs.GetInt("Sprinter") == 0)
        {
            sprinter.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Sprinter";
            UITime = Time.time;
            PlayerPrefs.SetInt("Sprinter", 1);
        }
        if (flexed && PlayerPrefs.GetInt("I'm Better Than You") == 0)
        {
            betterThanYou.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "I'm Better Than You";
            UITime = Time.time;
            PlayerPrefs.SetInt("I'm Better Than You", 1);
        }
        if (waved && PlayerPrefs.GetInt("Greetings") == 0)
        {
            greetings.color = achieved;
            achievementUICanvas.gameObject.SetActive(true);
            achievementName.text = "Greetings";
            UITime = Time.time;
            PlayerPrefs.SetInt("Greetings", 1);
        }

        if (manager != null)
        {
            //TODO: listen for singleplayer achievements here
            if(manager.totalScore >= 10 && PlayerPrefs.GetInt("Decade of Rooms") == 0)
            {
                decade.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Decade of Rooms";
                UITime = Time.time;
                PlayerPrefs.SetInt("Decade of Rooms", 1);
            }
            if (manager.totalScore >= 25 && PlayerPrefs.GetInt("Quarter-Century of Rooms") == 0)
            {
                quarter.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Quarter-Century of Rooms";
                UITime = Time.time;
                PlayerPrefs.SetInt("Quarter-Century of Rooms", 1);
            }
            if (manager.totalScore >= 50 && PlayerPrefs.GetInt("Half-Century of Rooms") == 0)
            {
                half.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Half-Century of Rooms";
                UITime = Time.time;
                PlayerPrefs.SetInt("Half-Century of Rooms", 1);
            }
            if (manager.totalScore >= 100 && PlayerPrefs.GetInt("Century of Rooms") == 0)
            {
                century.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Century of Rooms";
                UITime = Time.time;
                PlayerPrefs.SetInt("Century of Rooms", 1);
            }
            if (diedBy == 5 && PlayerPrefs.GetInt("Impaled") == 0)
            {
                //Died by spike
                impaled.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Impaled";
                UITime = Time.time;
                PlayerPrefs.SetInt("Impaled", 1);
            }
            else if(diedBy == 1 && PlayerPrefs.GetInt("Pitfall") == 0)
            {
                //Died by pit
                pitfall.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Pitfall";
                UITime = Time.time;
                PlayerPrefs.SetInt("Pitfall", 1);
            }
            else if(diedBy == 2 && PlayerPrefs.GetInt("Shot Dead") == 0)
            {
                //Died by arrow
                shotDead.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Shot Dead";
                UITime = Time.time;
                PlayerPrefs.SetInt("Shot Dead", 1);
            }
            else if(diedBy == 3 && PlayerPrefs.GetInt("Chopped") == 0)
            {
                //Died by saw
                chopped.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Chopped";
                UITime = Time.time;
                PlayerPrefs.SetInt("Chopped", 1);
            }
            else if(diedBy == 4 && PlayerPrefs.GetInt("Burned Alive") == 0)
            {
                //Died by fire
                quarter.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Burned Alive";
                UITime = Time.time;
                PlayerPrefs.SetInt("Burned Alive", 1);
            }
        }
        if (mrm != null)
        {
            //TODO: listen for multiplayer achievements here
            if (diedBy == 5 && PlayerPrefs.GetInt("A Friends Knife") == 0)
            {
                //Died by spike
                friendsKnife.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "A Friends Knife";
                UITime = Time.time;
                PlayerPrefs.SetInt("A Friends Knife", 1);
            }
            else if (diedBy == 1 && PlayerPrefs.GetInt("Yoshi") == 0)
            {
                //Died by pit
                yoshi.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Yoshi";
                UITime = Time.time;
                PlayerPrefs.SetInt("Yoshi", 1);
            }
            else if (diedBy == 2 && PlayerPrefs.GetInt("Betrayed") == 0)
            {
                //Died by arrow
                quarter.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Betrayed";
                UITime = Time.time;
                PlayerPrefs.SetInt("Betrayed", 1);
            }
            else if (diedBy == 3 && PlayerPrefs.GetInt("Prepared") == 0)
            {
                //Died by saw
                prepared.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Prepared";
                UITime = Time.time;
                PlayerPrefs.SetInt("Prepared", 1);
            }
            else if (diedBy == 4 && PlayerPrefs.GetInt("Burned At The Stake") == 0)
            {
                //Died by fire
                burnedAtStake.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Burned At The Stake";
                UITime = Time.time;
                PlayerPrefs.SetInt("Burned At The Stake", 1);
            }
            else if (diedBy == 6 && PlayerPrefs.GetInt("Encapsulated") == 0)
            {
                //Died by boss bubble
                encapsulated.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Encapsulated";
                UITime = Time.time;
                PlayerPrefs.SetInt("Encapsulated", 1);
            }
            else if (diedBy == 7 && PlayerPrefs.GetInt("Engulfed") == 0)
            {
                //Died by boss
                engulfed.color = achieved;
                achievementUICanvas.gameObject.SetActive(true);
                achievementName.text = "Engulfed";
                UITime = Time.time;
                PlayerPrefs.SetInt("Engulfed", 1);
            }
            //TODO: Listen for score in multiplayer being 100
            if(score == 100)
            {
                if (MultiplayerRunManager.Instance.endGameChoice)
                {
                    if(PlayerPrefs.GetInt("I Don't Need You") == 0)
                    {
                        dontNeedYou.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "I Don't Need You";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("I Don't Need You", 1);
                    }
                }else
                {
                    if(PlayerPrefs.GetInt("Treasure Hoarder") == 0)
                    {
                        treasureHorder.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "Treasure Hoarder";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("Treasure Hoarder", 1);
                    }
                }
            }
            if(score >= 75)
            {
                if (MultiplayerRunManager.Instance.endGameChoice)
                {
                    if (PlayerPrefs.GetInt("Boss Slayer") == 0)
                    {
                        bossSlayer.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "Boss Slayer";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("Boss Slayer", 1);
                    }
                }
                else
                {
                    if (PlayerPrefs.GetInt("Pile Of Gold") == 0)
                    {
                        pileOfGold.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "Pile Of Gold";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("Pile Of Gold", 1);
                    }
                }
            }
            if(score >= 50)
            {
                if (MultiplayerRunManager.Instance.endGameChoice)
                {
                    if (PlayerPrefs.GetInt("Boss Helper") == 0)
                    {
                        bossHelper.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "Boss Helper";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("Boss Helper", 1);
                    }
                }
                else
                {
                    if (PlayerPrefs.GetInt("Pocketful Of Gold") == 0)
                    {
                        pocketfulOfGold.color = achieved;
                        achievementUICanvas.gameObject.SetActive(true);
                        achievementName.text = "Pocketful Of Gold";
                        UITime = Time.time;
                        PlayerPrefs.SetInt("Pocketful Of Gold", 1);
                    }
                }
            }
        }
	}

    
}
