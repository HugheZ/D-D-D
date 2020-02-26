using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour {

    Rigidbody2D rb;
    public float speed = 10;
    public GameObject cursorTip;
    bool holdingSomething = false;
    Transform whatHolding;
    int layermask;
    DungeonBuilderManager manager;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        layermask = LayerMask.GetMask("Traps", "TrapsNoDmg");
        manager = DungeonBuilderManager.Instance;
    }
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(x, y) * speed;

        //will need to change for controller
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (holdingSomething)
            {
                //put down what you're holding
                holdingSomething = false;
                BoxCollider2D box = whatHolding.gameObject.GetComponent<BoxCollider2D>();
                if (box != null)
                    box.enabled = false;
                whatHolding.SetParent(manager.GetRoomTransform());
                whatHolding = null;
            }
            else
            {
                //raycast backwards
                RaycastHit2D hit = Physics2D.Raycast(cursorTip.transform.position, Vector2.zero, Mathf.Infinity, layermask);
                if (hit.collider != null)
                {
                    //make the hit a child of the cursor
                    holdingSomething = true;
                    whatHolding = hit.transform;
                    BoxCollider2D box = whatHolding.gameObject.GetComponent<BoxCollider2D>();
                    if (box != null)
                        box.enabled = false;
                    whatHolding.SetParent(transform);
                    
                }
            }
        }
    }
}
