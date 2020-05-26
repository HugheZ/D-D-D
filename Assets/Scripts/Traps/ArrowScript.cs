using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool facingRight;
    public bool facingNorth;
    public bool facingSouth;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) gameObject.SetActive(false); //if collided with player, just disable
        else //else play clip
        {
            GetComponent<Animator>().SetTrigger("Stab");
            //disable collider
            GetComponent<BoxCollider2D>().enabled = false;
            //freeze rigidbody
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void ResetForShooting()
    {
        gameObject.SetActive(false);
        //unfreeze collision logic and set inactive
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable()
    {
        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            GetComponent<SpriteRenderer>().flipY = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(2, 0);
        }
        else if (facingSouth)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<SpriteRenderer>().flipY = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -2);
        }
        else if (facingNorth)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            GetComponent<SpriteRenderer>().flipY = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 2);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            GetComponent<SpriteRenderer>().flipY = true;
            GetComponent<Rigidbody2D>().velocity = new Vector2(-2, 0);
        }
    }
}
