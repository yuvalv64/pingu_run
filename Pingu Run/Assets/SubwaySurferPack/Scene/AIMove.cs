using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour {
    //declare vars for AISpanwer manager script
    private AISpawner m_AIManager;

    //declare vars for moving and turning
    private bool m_hasTarget = false; // have we lock down on target?
    private bool m_isTurning;

    //vars for the curent waypoint
    private Vector3 m_wayPoint; // for defer between the two waypoints
    private Vector3 m_lastWaypoint = new Vector3(0f, 0f, 0f);

    //going to use this to set the animation speed
    private Animator m_animator;
    private float m_speed;
    /*
    private float m_scale;

    private Collider m_collider;
    private RaycastHit m_hit;
    */
	// Use this for initialization
	void Start () {
        m_AIManager = transform.parent.GetComponentInParent<AISpawner>();
        m_animator = GetComponent<Animator>();

        setUpNPC();
		
	}

    private void setUpNPC()
    {
        //randomally scale each NPC - the size of the fish
        float m_scale = UnityEngine.Random.Range(0f, 4f);
        transform.localScale += new Vector3(m_scale * 1.5f, m_scale, m_scale);
    }

    // Update is called once per frame
    void Update () {

        if (!m_hasTarget)
        {
            m_hasTarget = CanFindTarget();
        }
        else
        {
            RotateNPC(m_wayPoint,m_speed);
            transform.position = Vector3.MoveTowards(transform.position,m_wayPoint,
                m_speed *Time.deltaTime);
         
        }
        // the NPC   REACHES THE TARGET POINT
        if (transform.position==m_wayPoint)
        {
            m_hasTarget = false;
        }

	}

    bool CanFindTarget(float start = 1f, float end =7f )
    {
        m_wayPoint = m_AIManager.RandomWayPoint();

        //make sure we not set the same waypoint twice
        if (m_lastWaypoint ==m_wayPoint)
        {
            m_wayPoint = m_AIManager.RandomWayPoint();
            return false;
        }
        else
        {
            // set the last way point to be the new one
            m_lastWaypoint = m_wayPoint;
            m_speed = UnityEngine.Random.Range(start, end);
            // change the animator speed for the Defeqult Level - TIP
            m_animator.speed = m_speed;

            return true;
        }
    }

    void RotateNPC(Vector3 waypoint , float currentSpeed)
    {
        float TrunSpeed =currentSpeed * UnityEngine.Random.Range(1f,3f);

        
        Vector3 LookAt = waypoint - this.transform.position;

        //get a a new direction to look at for target
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(LookAt), TrunSpeed * Time.deltaTime);
    }
}
