using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slasher : MonoBehaviour
{
    PlayerMovement mvr;
    Animator anim;
    bool flexing;
    bool isSwinging;

    //Hitboxes
    public BoxCollider2D upHB;
    public BoxCollider2D rightHB;
    public BoxCollider2D downHB;
    public BoxCollider2D leftHB;
    public AudioSource PlayerSource;
    public AudioClip whoosh;

    // Start is called before the first frame update
    void Start()
    {
        mvr = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        isSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        //check for clicks
        if (Input.GetMouseButtonDown(0) && !isSwinging && !mvr.IsPosing())
        {
            isSwinging = true;
            //disable
            DisableHitboxes();

            //swing, which will enable collider
            anim.SetTrigger("slash");

            //notify mover
            mvr.DisableMove();
            PlayerSource.PlayOneShot(whoosh);

        }
    }

    /// <summary>
    /// Enables up hitbox
    /// </summary>
    public void EnableHitboxUp()
    {
        upHB.enabled = true;
    }

    /// <summary>
    /// Enables side hitbox
    /// </summary>
    public void EnableHitboxSide()
    {
        if (mvr.IsFacingLeft()) leftHB.enabled = true;
        else rightHB.enabled = true;
    }

    /// <summary>
    /// Enables down hitbox
    /// </summary>
    public void EnableHitboxDown()
    {
        downHB.enabled = true;
    }

    /// <summary>
    /// Disables attack hitboxes
    /// </summary>
    void DisableHitboxes()
    {
        upHB.enabled = false;
        rightHB.enabled = false;
        downHB.enabled = false;
        leftHB.enabled = false;
    }

    /// <summary>
    /// Ends the slash animation
    /// </summary>
    public void EndSlash()
    {
        isSwinging = false;
        mvr.EnableMove();
        //disable colliders
        DisableHitboxes();

        //check for flexing
        mvr.CheckFlex();
    }
}
