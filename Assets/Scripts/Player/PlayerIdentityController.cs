using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerIdentityController : NetworkBehaviour {
    
    [SyncVar(hook = "SetAnimator")]
    public int playerAnimIndex = 0;

    [SyncVar]
    public string name; // the player's name

    public Text nameSpace; //place for the user's name

    [SyncVar]
    public int UID; //the player's unique identifier, GUARANTEED the same across all clients and host

    public override void OnStartLocalPlayer()
    {
        //read player pref for anim index, do nothing if not found
        int anim = PlayerPrefs.GetInt("PlayerAnim", -1);
        //if we have a pref, tell the host to load that one instead
        if (anim >= 0)
        {
            CmdSetAnimatorCustom(anim);
        }
        else
        {
            Debug.LogError("Attempt assert animator that is not loaded");
        }
    }

    /// <summary>
    /// Sets the animator of this player
    /// </summary>
    /// <param name="index">Index into globally loaded animator list</param>
    public void SetAnimator(int index)
    {
        //set animation index if our controllers list has an entry for it, default to standard animation index
        //NOTE: on update from custom, may need to run StartPlayback();
        playerAnimIndex = NetManScript.Instance.controllers.Count > index ? index : 0;
        GetComponent<Animator>().runtimeAnimatorController = NetManScript.Instance.controllers[index] as RuntimeAnimatorController;
    }

    /// <summary>
    /// On client start, set animator to given index, should be set by default
    /// </summary>
    public override void OnStartClient()
    {
        SetAnimator(playerAnimIndex);
        nameSpace.text = name;
    }

    /// <summary>
    /// Sets this specific player's animator to a custom one
    /// </summary>
    /// <param name="index">The index to use as the animator</param>
    [Command]
    void CmdSetAnimatorCustom(int index)
    {
        if (NetManScript.Instance.controllers.Count <= index)
        {
            Debug.LogError("Attempt assert animator that server does not have");
        }
        else
        {
            playerAnimIndex = index;
        }
    }
}
