using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlasherLocal : MonoBehaviour
{
    PlayerMovementLocal mvr;
    Animator anim;
    public bool isSwinging;

    //damage per player swing
    public float damagePerSwing;

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
        mvr = GetComponent<PlayerMovementLocal>();
        anim = GetComponent<Animator>();
        isSwinging = false;
    }

    // Update is called once per frame
    void Update()
    {
        //check for clicks
        if ((Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Joystick2Button0)) && !isSwinging && !mvr.IsPosing())
        {
            RpcSwing();
        }
    }

    /// <summary>
    /// Swings the player's axe
    /// </summary>
    void RpcSwing()
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
        mvr.SetMove();
        //disable colliders
        DisableHitboxes();

        //check for flexing
        mvr.CheckFlex();
    }
}
