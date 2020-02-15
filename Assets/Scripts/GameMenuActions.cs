using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for button actions for menus specific to in-game functionality
/// </summary>
public class GameMenuActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Returns to the menu
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
