using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConfigMenu : MonoBehaviour {

    [Tooltip("Skin widgets, expects setup as image > button > animator image. Be sure to set in order that the prefabs are saved.")]
    public List<Image> skinWidgets; // the list of skin widgets for animators

    int selectedAnim = 0;

    /// <summary>
    /// On start, enable the image of the selected animator, 0 default
    /// </summary>
    private void Start()
    {
        //set selected
        skinWidgets[0].GetComponentInChildren<Button>().Select();

        selectedAnim = PlayerPrefs.GetInt("PlayerAnim", 0);
        skinWidgets[selectedAnim].enabled = true;

        //enable buttons if you unlocked a skin
        int gUnlocked = PlayerPrefs.GetInt("Treasure Hoarder", 0);

        if(gUnlocked == 1)
        {
            skinWidgets[skinWidgets.Count - 1].GetComponentInChildren<Button>().interactable = true;
        }
    }

    /// <summary>
    /// Selects new animator and informs player pref
    /// </summary>
    /// <param name="index"></param>
    public void SelectAnimator(int index)
    {
        skinWidgets[selectedAnim].enabled = false;
        selectedAnim = index;
        skinWidgets[selectedAnim].enabled = true;

        PlayerPrefs.SetInt("PlayerAnim", selectedAnim);
    }
}
