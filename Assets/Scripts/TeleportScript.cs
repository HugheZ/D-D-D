using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
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
            mman.NextRoom(collision.gameObject);
        }
    }
}
