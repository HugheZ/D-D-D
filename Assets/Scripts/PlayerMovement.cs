using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    public float speed;
    Rigidbody2D rb;
    SpriteRenderer rend;

    bool facingLeft;
    bool flexing;
    bool hi;

    public bool canMove;
    public AudioSource PlayerSound;
    public AudioClip leftFoot;
    public AudioClip rightFoot;
    public AudioClip hello;
    public AudioClip FLEXBROTHER;
    public bool foot;
    float lastFoot;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();

        facingLeft = false;
        flexing = false;
        canMove = true;
        hi = false;
        foot = true;
        lastFoot = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float x = 0f;
        float y = 0f;

        //if we can move, allow movement inputs
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerSound.PlayOneShot(FLEXBROTHER);
                anim.SetTrigger("flexBegin");
            }
            else if (Input.GetKey(KeyCode.R))
            {
                flexing = true;
                anim.SetBool("isFlexing", flexing);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                flexing = false;
                anim.SetBool("isFlexing", flexing);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                if (!hi)
                {
                    PlayerSound.PlayOneShot(hello);
                }
                anim.SetBool("hi", true);
                hi = true;
                //TODO: play sound
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                anim.SetBool("hi", false);
                hi = false;
            }
            else
            {
                //set velocities
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");

                //check flip
                //flip if going majority left
                if (x < -.01f && y < .5f && y > -.5f)
                {
                    //if we were facing right, flip the sprite
                    if (!facingLeft)
                    {
                        facingLeft = true;
                        rend.flipX = true;
                    }
                }
                else if (x > .01f || (x > -.5f && (y > .5f || y < -.5f)))
                {
                    //flip if we were going left
                    if (facingLeft)
                    {
                        facingLeft = false;
                        rend.flipX = false;
                    }
                }

                if (2 * x + y != 0 && ((Time.time-lastFoot)>.25))
                {
                    if (foot)
                    {
                        lastFoot = Time.time;
                        PlayerSound.PlayOneShot(leftFoot);
                    }
                    else
                    {
                        lastFoot = Time.time;
                        PlayerSound.PlayOneShot(rightFoot);
                    }
                }
            }
        }

        

        //set anim controllers
        anim.SetFloat("x", x);
        anim.SetFloat("y", y);

        //move by speed
        rb.velocity = new Vector2(x, y) * speed;
    }

    /// <summary>
    /// Disables movement
    /// </summary>
    public void DisableMove()
    {
        canMove = false;
    }

    /// <summary>
    /// Allows re-enabling mvoement from external source
    /// </summary>
    public void EnableMove()
    {
        canMove = true;
    }


    /// <summary>
    /// Checks if the player is trying to flex
    /// </summary>
    public void CheckFlex()
    {
        if (Input.GetKey(KeyCode.R))
        {
            flexing = true;
            anim.SetTrigger("flexBegin");
        }
    }

    /// <summary>
    /// Gets the face direction
    /// </summary>
    /// <returns>True if facing left, False if not</returns>
    public bool IsFacingLeft()
    {
        return facingLeft;
    }

    /// <summary>
    /// Tells whether the player is doing a pose or not
    /// </summary>
    /// <returns>Bool true if saying hi or flexing, false otherwise</returns>
    public bool IsPosing()
    {
        return flexing || hi;
    }
}
