using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerIdentity : MonoBehaviour {

    private int _ID;
    private bool _set = false;

    public int playerAnimIndex = 1;

    public string name;

    public int UID; //the player's unique identifier, GUARANTEED the same across all clients and host

    public int ID
    {
        get
        {
            if (_set)
            {
                return _ID;
            }
            else throw new System.Exception("ID was not yet set");
        }
        set
        {
            _ID = value;
            _set = true;
        }
    }

    private void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = NetManScript.Instance.controllers[playerAnimIndex] as RuntimeAnimatorController;
    }
}
