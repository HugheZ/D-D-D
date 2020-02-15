using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    ManagerScript manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        manager = ManagerScript.Instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        manager.NextRoom();
    }
}
