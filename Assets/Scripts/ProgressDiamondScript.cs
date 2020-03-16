using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDiamondScript : MonoBehaviour {

    float[] progresses = { 0, 0, 0, 0 };
    public Image player1, player2, player3, player4;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeProgress(int playerIndex, float progress)
    {
        progresses[playerIndex] += progress;
        if(playerIndex == 0)
        {
            player1.fillAmount = progresses[playerIndex];
        }else if (playerIndex == 1)
        {
            player2.fillAmount = progresses[playerIndex];
        }
        else if (playerIndex == 2)
        {
            player3.fillAmount = progresses[playerIndex];
        }
        else if (playerIndex == 3)
        {
            player4.fillAmount = progresses[playerIndex];
        }
    }
}
