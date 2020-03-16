using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    Animator anim;
    public float speed;
    Rigidbody2D rb;
    SpriteRenderer rend;

    [SyncVar]
    Vector2 receivedPosn;
    Vector2 oldPosn;

    [SyncVar]
    float receivedRot;
    float oldRot;

    //updator
    Coroutine updator;

    //update time
    [SerializeField]
    float interval;

    [SyncVar(hook = "FlipSpriteRend")]
    bool facingLeft;
    [SyncVar(hook = "ToggleFlex")]
    bool flexing;
    [SyncVar(hook = "ToggleHi")]
    bool hi;

    //Whether flex was played
    bool playedFlex = false;

    //Whether hello was played
    bool helloPlayed = false;

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

        //if local player, update position
        if (isLocalPlayer) updator = StartCoroutine(UpdatePosn());
    }

    // Update is called once per frame
    void Update()
    {
        float x = 0f;
        float y = 0f;

        //if we can move, allow movement inputs
        if (canMove)
        {
            if (isLocalPlayer)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    flexing = true;
                    if (!isServer) CmdUpdateAnimControl(1, true);
                    ToggleFlex(flexing);
                }
                //else if (Input.GetKey(KeyCode.R))
                //{
                //    flexing = true;
                //    anim.SetBool("isFlexing", flexing);
                //}
                else if (Input.GetKeyUp(KeyCode.R))
                {
                    flexing = false;
                    if (!isServer) CmdUpdateAnimControl(1, false);
                    ToggleFlex(flexing);
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    hi = true;
                    if (!isServer) CmdUpdateAnimControl(2, true);
                    ToggleHi(hi);
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    hi = false;
                    if(!isServer) CmdUpdateAnimControl(2, false);
                    ToggleHi(hi);
                }
                else if(!IsPosing())
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
                            if (!isServer) CmdUpdateAnimControl(0,true);
                            facingLeft = true;
                            FlipSpriteRend(facingLeft);
                        }
                    }
                    else if (x > .01f || (x > -.5f && (y > .5f || y < -.5f)))
                    {
                        //flip if we were going left
                        if (facingLeft)
                        {
                            if (!isServer) CmdUpdateAnimControl(0, false);
                            facingLeft = false;
                            FlipSpriteRend(facingLeft);
                        }
                    }
                }

                //move by speed
                rb.velocity = new Vector2(x, y) * speed;
            }
            else //is not local, simply lerp remote to move the player
            {
                LerpRemote();

                //set x values
                x = rb.velocity.x;
                y = rb.velocity.y;
            }

            //play sound
            if (2 * x + y != 0 && ((Time.time - lastFoot) > .25))
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
    /// Lerps the player through the physics engine
    /// </summary>
    void LerpRemote()
    {
        if (Vector2.Distance(rb.position, receivedPosn) > .1f)
            rb.MovePosition(Vector2.Lerp(transform.position, receivedPosn, 10 * Time.deltaTime));
        if (Mathf.Abs(Mathf.DeltaAngle(rb.rotation, receivedRot)) > .1f)
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, receivedRot, 10 * Time.deltaTime));
    }

    /// <summary>
    /// Tells server to update syncvar position
    /// </summary>
    /// <param name="pos">Vector2 position for this player</param>
    [Command]
    void CmdUpdatePos(Vector2 pos)
    {
        receivedPosn = pos;
    }

    /// <summary>
    /// Tells server to update syncvar rotation
    /// </summary>
    /// <param name="rot">Float degree rotation for this player</param>
    [Command]
    void CmdUpdateRot(float rot)
    {
        receivedRot = rot;
    }

    /// <summary>
    /// Updates the player position by global time value
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdatePosn()
    {
        while (true)
        {
            if (rb.position != oldPosn)
            {
                oldPosn = rb.position;
                CmdUpdatePos(rb.position);
            }
            if (Mathf.Abs(Mathf.DeltaAngle(rb.rotation, oldRot)) > .1f)
            {
                oldRot = rb.rotation;
                CmdUpdateRot(rb.rotation);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Commands the server to flip the sprite
    /// </summary>
    /// <param name="control">Which control to flip: 0 is renderer flip, 1 is flexing, 2 is hi</param>
    /// <param name="val">Whether the control should be true or false</param>
    [Command]
    void CmdUpdateAnimControl(int control, bool val)
    {
        switch (control)
        {
            case 0:
                facingLeft = val;
                break;
            case 1:
                flexing = val;
                break;
            case 2:
                hi = val;
                break;
            default:
                break;
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
    void ToggleFlex(bool flex)
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
        if (hello)
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
