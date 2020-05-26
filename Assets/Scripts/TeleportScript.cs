using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeleportScript : NetworkBehaviour
{
    ManagerScript manager;
    MultiplayerRunManager mman;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        manager = ManagerScript.Instance;
        mman = MultiplayerRunManager.Instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (manager != null)
        {
            manager.NextRoom();
        }
        else
        {
            print("Bad touch! No touchy!");
            GameObject obj = collision.gameObject.CompareTag("GHOST") ? collision.transform.parent.gameObject : collision.gameObject;
            CmdCallNextRoon(obj);
        }
    }
    [Command]
    void CmdCallNextRoon(GameObject player)
    {
        mman.NextRoom(player);
        //RpcCallNextRoon(player);
    }
    //[ClientRpc]
    //void RpcCallNextRoon(GameObject player)
    //{
    //    mman.NextRoom(player);
    //}
}
