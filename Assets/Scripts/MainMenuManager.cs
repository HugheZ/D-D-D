using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    Animator anim; //main menu animator
    public Image overlay; //the flash overlay
    public GameObject UI; //the actual UI components
    public GameObject AboutCanvas; //the canvas used for the About popup
    public GameObject configCanvas; //player config canvas
    public AudioSource MenuAudio;
    public AudioClip click;
    public Button firstButton;
    public EventSystem ev;

    private string selectedGame;

    //stuff for Singleton
    private static MainMenuManager _instance = null;
    public static MainMenuManager Instance
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


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        selectedGame = "EndlessScene";
    }

    // Update is called once per frame
    void Update()
    {
        //if player skips intro
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("skip");
            EnableUI();
            DisableOverlay();
        }
    }

    /// <summary>
    /// Disables the color overlay
    /// </summary>
    public void DisableOverlay()
    {
        overlay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables the UI functionality
    /// </summary>
    public void EnableUI()
    {
        UI.SetActive(true);
        firstButton.Select();
    }


    //////Button Functions
    

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        MenuAudio.PlayOneShot(click);
        Application.Quit();
    }


    /// <summary>
    /// Starts endless singleplayer TODO
    /// </summary>
    public void StartEndless()
    {
        selectedGame = "EndlessScene";
        anim.SetTrigger("start");
        MenuAudio.PlayOneShot(click);
        SceneManager.LoadScene("EndlessScene");
    }


    /// <summary>
    /// Starts multiplayer connection lobby
    /// </summary>
    public void StartMultiplayerSearch()
    {
        selectedGame = "MultiplayerGame";
        anim.SetTrigger("start");
        MenuAudio.PlayOneShot(click);
        SceneManager.LoadScene("MultiplayerRun");
        //throw new Exception("Not yet implemented");
    }

    /// <summary>
    /// Shows the about section TODO
    /// </summary>
    public void ShowAbout()
    {
        AboutCanvas.SetActive(true);
        UI.SetActive(false);
        MenuAudio.PlayOneShot(click);
        //SceneManager.LoadScene("Experimental");
    }

    /// <summary>
    /// Hides the about section TODO
    /// </summary>
    public void HideAbout()
    {
        AboutCanvas.SetActive(false);
        UI.SetActive(true);
        MenuAudio.PlayOneShot(click);
        firstButton.Select();
    }

    public void GoToConfig()
    {
        SceneManager.LoadScene("PlayerConfig");
    }

    /// <summary>
    /// Starts the selected game from the stored value
    /// </summary>
    public void StartSelectedGame()
    {
        SceneManager.LoadScene(selectedGame);
    }

    public void StartBoss()
    {
        SceneManager.LoadScene("Experimental");
    }

    public void OnDisable()
    {
        AcheivementScript.Instance.diedBy = 0;
    }

    public void loadAchievementScreen()
    {
        AcheivementScript.Instance.achievementPage1.SetActive(true);
        SceneManager.LoadScene("AchievementScene");
    }

    
}
