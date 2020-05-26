using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnCollisionEnter2D()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<AudioSource>().Play();
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<ParticleSystem>().Play();
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnDisable()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        gameObject.SetActive(false);
    }
}
