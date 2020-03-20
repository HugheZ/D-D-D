using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerIdentityController : NetworkBehaviour {
    
    [SyncVar(hook = "ChangeAnimatorController")]
    public int index = -1;

    public void ChangeAnimatorController(int animIndex)
    {
        GetComponent<Animator>().runtimeAnimatorController = NetManScript.Instance.controllers[animIndex] as RuntimeAnimatorController;
        GetComponent<Animator>().StartPlayback();
    }
}
