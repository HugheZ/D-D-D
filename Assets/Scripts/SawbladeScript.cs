using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeScript : MonoBehaviour
{
    Rigidbody2D rb;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!isActive)
            GetComponent<CircleCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        rb.rotation += 2.0f;
    }

    void flipActive()
    {
        isActive = !isActive;
        if (!isActive)
            GetComponent<CircleCollider2D>().enabled = false;
        else
            GetComponent<CircleCollider2D>().enabled = true;
    }
}
