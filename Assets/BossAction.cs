using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : MonoBehaviour {
    Animator anim; //animator controller
    Vector3 nextPosn; //next position to move to
    public float maxSlideDistance; //maximum distance to slide to
    public float slideTime;
    public float maxJumpDistance; //maximum distance to jump to
    public float jumpTime;
    [SerializeField]
    LayerMask wallLayer; //layer for walls
    float bossRad; //radius of boss' collider
    [SerializeField]
    Bubble bubble; //bubble prototype
    [SerializeField]
    List<Vector2> shootPoints; //transform spot for shooting
    [SerializeField]
    float shootSpeed; //speed to shoot points
    Rigidbody2D rb;

    Coroutine moveAction; //current move action

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        bossRad = GetComponent<CircleCollider2D>().radius;
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Logic for jumping
    /// </summary>
    public void Jump()
    {
        Vector2 newPosn = GenerateNextJump();
        if (moveAction != null) StopCoroutine(moveAction);
        moveAction = StartCoroutine(_MV(newPosn, jumpTime));
    }

    /// <summary>
    /// Logic for moving
    /// </summary>
    public void Move()
    {
        Vector2 newPosn = GenerateNextPosition();
        if (moveAction != null) StopCoroutine(moveAction);
        moveAction = StartCoroutine(_MV(newPosn, slideTime));
    }

    IEnumerator _MV(Vector2 position, float time)
    {
        Vector2 spd;
        float curTime = 0f;

        while(Vector2.Distance(transform.position, position) > .1f)
        {
            curTime += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, position, curTime / time);
            print("Pos: " + position + "\nDist: " + Vector2.Distance(transform.position, position));
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Logic for shooting slime balls
    /// </summary>
    public void Shoot()
    {
        foreach (Vector2 point in shootPoints)
        {
            Bubble bub = Instantiate(bubble, transform.TransformPoint(point), Quaternion.identity);
            bub.GetComponent<Rigidbody2D>().velocity = (transform.TransformPoint(point) - transform.position) * shootSpeed;
        }
    }

    /// <summary>
    /// Generates the next position to move to
    /// </summary>
    public Vector2 GenerateNextPosition()
    {
        Vector2 newPoint;
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        //raycast to see if we can go there, offset by == (    bossRad-------)-------slide------->  |wall|
        RaycastHit2D ray = Physics2D.Raycast(transform.position, dir, maxSlideDistance + bossRad, wallLayer);
        //if the hit was true, reduce direction
        if (ray)
        {
            newPoint = ray.point - dir * bossRad;
        }
        else
        {
            newPoint = new Vector2(transform.position.x, transform.position.y) + (dir * maxSlideDistance);
        }

        return newPoint;
    }

    /// <summary>
    /// Generates the next jump position
    /// </summary>
    public Vector2 GenerateNextJump()
    {
        Vector2 newPoint;
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        //raycast to see if we can go there, offset by == (    bossRad-------)-------slide------->  |wall|
        RaycastHit2D ray = Physics2D.Raycast(transform.position, dir, maxJumpDistance + bossRad, wallLayer);
        //if the hit was true, loop until we can jump to this point
        if (ray)
        {
            newPoint = ray.point - dir * bossRad;
        }
        else
        {
            newPoint = new Vector2(transform.position.x, transform.position.y) + (dir * maxJumpDistance);
        }
        

        return newPoint;
    }
}
