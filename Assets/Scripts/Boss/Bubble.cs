using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        //start particle
        GetComponent<ParticleSystem>().Play();
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<AudioSource>().Play();
    }
}
