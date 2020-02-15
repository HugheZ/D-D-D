using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    BoxCollider2D spikeBox;
    public AudioSource spikeAudio;
    public Camera camera;
    Transform tform;

    // Start is called before the first frame update
    void Start()
    {
        spikeBox = GetComponent<BoxCollider2D>();
        spikeBox.isTrigger = true;
        tform = gameObject.GetComponent<Transform>();
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (distToCam(tform.position.x, tform.position.y)>9.0)
        {
            gameObject.GetComponent<AudioSource>().mute = true;
        }
        else if (distToCam(tform.position.x, tform.position.y) < 9)
        {
            gameObject.GetComponent<AudioSource>().mute = false;
        }
    }

    public void setSpikesInactive()
    {
        spikeBox.enabled = false;
    }

    public void setSpikesActive()
    {
        spikeAudio.PlayOneShot(spikeAudio.clip);
        spikeBox.enabled = true;
    }

    public void OnBecameVisible()
    {

        gameObject.GetComponent<AudioSource>().mute = false;

    }
    public void OnBecameInvisible()
    {
        gameObject.GetComponent<AudioSource>().mute = true;
    }

    public float distToCam(float x, float y)
    {
        Transform ct = camera.transform;
        float cx = ct.position.x;
        float cy = ct.position.y;

        return (Mathf.Sqrt(((x - cx) * (x - cx)) + ((y - cy) * (y - cy))));
    }
}
