using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldZone : MonoBehaviour {

    /// <summary>
    /// On trigger enter override
    /// When a player enters, adds it to its manager's list
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if is a player, add it to the list
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GoldRoomManager.Instance) GoldRoomManager.Instance.NotifyOfPlayerZoneInteraction(true, collision.gameObject);
            print("Player entered");
        }
    }

    /// <summary>
    /// On trigger exit override
    /// When a player exits, remove it from its manager's list
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if is a player, add it to the list
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GoldRoomManager.Instance) GoldRoomManager.Instance.NotifyOfPlayerZoneInteraction(false, collision.gameObject);
            print("Player exited");
        }
    }

}
