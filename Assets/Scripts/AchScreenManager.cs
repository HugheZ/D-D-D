using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchScreenManager : MonoBehaviour {
    bool foundBtn = false;
    public Button backBtn;
    public Button page1Btn;
    public Button page2Btn;
    public AcheivementScript ach;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(ach == null)
        {
            ach = AcheivementScript.Instance;
        }
    }

    public void onPage1ButtoneClick()
    {
        ach.achievementPage1.SetActive(true);
        ach.achievementPage2.SetActive(false);
        ach.achievementPage3.SetActive(false);
    }

    public void onPage2ButtonClick()
    {
        ach.achievementPage1.SetActive(false);
        ach.achievementPage2.SetActive(true);
        ach.achievementPage3.SetActive(false);
    }

    public void onPage3ButtoneClick()
    {
        ach.achievementPage1.SetActive(false);
        ach.achievementPage2.SetActive(false);
        ach.achievementPage3.SetActive(true);
    }

    public void loadMainMenu()
    {
        ach.achievementPage1.SetActive(false);
        ach.achievementPage2.SetActive(false);
        ach.achievementPage3.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
