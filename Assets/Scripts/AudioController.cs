using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    //public Camera camera;
    Transform tform;

    // Start is called before the first frame update
    void Start()
    {
        tform = gameObject.GetComponent<Transform>();
        //camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (distToCam(tform.position.x, tform.position.y) > 9.0)
        {
            gameObject.GetComponent<AudioSource>().mute = true;
        }
        else if (distToCam(tform.position.x, tform.position.y) < 9.0)
        {
            gameObject.GetComponent<AudioSource>().mute = false;
        }
    }*/

    /*private void OnBecameInvisible()
    {
        //camera = FindObjectOfType<Camera>();
        if (distToCam(tform.position.x, tform.position.y) > 9.0)
        {
            gameObject.GetComponent<AudioSource>().mute = true;
        }
    }*/

    /*private void OnBecameVisible()
    {
        //camera = FindObjectOfType<Camera>();
        if (distToCam(tform.position.x, tform.position.y) < 9.0)
        {
            gameObject.GetComponent<AudioSource>().mute = false;
        }
    }*/
    /*public float distToCam(float x, float y)
    {
        Transform ct = camera.transform;
        float cx = ct.position.x;
        float cy = ct.position.y;

        return (Mathf.Sqrt(((x - cx) * (x - cx)) + ((y - cy) * (y - cy))));
    }*/
}
