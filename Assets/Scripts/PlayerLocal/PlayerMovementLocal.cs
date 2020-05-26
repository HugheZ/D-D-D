using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementLocal : MonoBehaviour
{
    Animator anim;
    public float speed;
    Rigidbody2D rb;
    SpriteRenderer rend;

    float receivedRot;
    float oldRot;

    //updator
    Coroutine updator;

    //update time
    [SerializeField]
    float interval;

    bool facingLeft;
    bool flexing;
    bool hi;

    //Whether flex was played
    bool playedFlex = false;

    //Whether hello was played
    bool helloPlayed = false;

    public bool canMove;
    public bool isPosing = false;
    public AudioSource PlayerSound;
    public AudioClip leftFoot;
    public AudioClip rightFoot;
    public AudioClip hello;
    public AudioClip FLEXBROTHER;
    public bool foot;
    float lastFoot;

    AcheivementScript acheivements;

    public Camera playerCamera;

    //boolean for controlling death collisions
    public bool dead = false;

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

        acheivements = AcheivementScript.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        isPosing = IsPosing();
        float x = 0f;
        float y = 0f;

        //if we can move, allow movement inputs
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Joystick2Button5) && !dead)
            {
                flexing = true;
                ToggleFlex(flexing);
                if(acheivements) acheivements.flexed = true;
            }
            //else if (Input.GetKey(KeyCode.R))
            //{
            //    flexing = true;
            //    anim.SetBool("isFlexing", flexing);
            //}
            else if (Input.GetKeyUp(KeyCode.Y) || Input.GetKeyUp(KeyCode.Joystick2Button5) && !dead)
            {
                flexing = false;
                ToggleFlex(flexing);
                if(acheivements) acheivements.flexed = true;
                    
            }
            else if (Input.GetKey(KeyCode.O) || Input.GetKeyDown(KeyCode.Joystick2Button4) && !dead)
            {
                hi = true;
                ToggleHi(hi);
            }
            else if (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.Joystick2Button4) && !dead)
            {
                hi = false;
                ToggleHi(hi);
                if(acheivements) acheivements.waved = true;
            }
            else if(!IsPosing())
            {
                //set velocities
                x = Input.GetAxis("HorizontalLocal");
                y = Input.GetAxis("VerticalLocal");

                //check flip
                //flip if going majority left
                if (x < -.01f && y < .5f && y > -.5f)
                {
                    //if we were facing right, flip the sprite
                    if (!facingLeft)
                    {
                        facingLeft = true;
                        FlipSpriteRend(facingLeft);
                    }
                }
                else if (x > .01f || (x > -.5f && (y > .5f || y < -.5f)))
                {
                    //flip if we were going left
                    if (facingLeft)
                    {
                        facingLeft = false;
                        FlipSpriteRend(facingLeft);
                    }
                }
            }

            //move by speed
            rb.velocity = new Vector2(x, y) * speed;


            //play sound
            if (!dead && 2 * x + y != 0 && ((Time.time - lastFoot) > .25))
            {
                if (foot)
                {
                    foot = false;
                    lastFoot = Time.time;
                    PlayerSound.PlayOneShot(leftFoot);
                }
                else
                {
                    foot = true;
                    lastFoot = Time.time;
                    PlayerSound.PlayOneShot(rightFoot);
                }
            }


            //set anim controllers
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
        }
    }

    /// <summary>
    /// Flips the sprite renderer
    /// </summary>
    /// <param name="flip">Whether the renderer should be flipped or not</param>
    void FlipSpriteRend(bool flip)
    {
        rend.flipX = flip;
        print(flip);
    }

    /// <summary>
    /// Toggles the flex field
    /// </summary>
    /// <param name="flex">Whether the player is flexing</param>
    public void ToggleFlex(bool flex)
    {
        if (flex)
        {
            //play sound if not yet played
            if (!playedFlex)
            {
                playedFlex = true;
                PlayerSound.PlayOneShot(FLEXBROTHER);
            }
            anim.SetTrigger("flexBegin");
        }
        else
        {
            playedFlex = false;
            anim.ResetTrigger("flexBegin");
        }

        anim.SetBool("isFlexing", flex);
    }

    /// <summary>
    /// Toggles the hi function
    /// </summary>
    /// <param name="yo">whether the player is saying hello</param>
    void ToggleHi(bool yo)
    {
        if (yo)
        {
            //play sound if not yet done
            if (!helloPlayed)
            {
                PlayerSound.PlayOneShot(hello);
            }

            helloPlayed = true;
        }
        else
        {
            helloPlayed = false;
        }

        anim.SetBool("hi", yo);
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
    public void SetMove()
    {
        canMove = true;
    }


    /// <summary>
    /// Checks if the player is trying to flex
    /// </summary>
    public void CheckFlex()
    {
        if (Input.GetKey(KeyCode.Y))
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

    /// <summary>
    /// Resets the animation state of this player
    /// </summary>
    public void ResetAnimationState()
    {
        ToggleHi(false);
        ToggleFlex(false);
        hi = false;
        flexing = false;
    }
}
