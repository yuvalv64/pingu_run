using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour {
    public Vector3 temporaryTarget;
    private Animator anim;

    private const float LANE_DISTANCEC = 2.5f;
    private const float TURN_SPEED = 0.05f;

    [SerializeField]
	public Transform _destination;

	NavMeshAgent _navMeshAgent;

    //Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    //Movement
    private float jumpForce = 4f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1; //0= Left , 1= Middle, 2= Right
    private CharacterController controller;

    // Update is called once per frame
    void Update () {
        anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        temporaryTarget = GameObject.Find("LevelManager").GetComponent<LevelManager>().m_wayPoint;

        //IsRunning or Not
        if (!(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>().isRunning))
        {
            return;
        }

        StartRunning();

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            //Change the Modifier text
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = temporaryTarget.z * Vector3.forward;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCEC;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCEC;
        }

        //calculate our move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;


        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);
        //calaulate Y
        if (isGrounded)
        { //if grounded
            verticalVelocity = -0.1f;
            /*
			if (MobileInput.Instance.SwipeUp||(Input.GetKeyDown(KeyCode.UpArrow))) {
				//Jump
				anim.SetTrigger ("Jump");
				verticalVelocity = jumpForce;
			} 
					
				if (MobileInput.Instance.SwipeDown ||(Input.GetKeyDown(KeyCode.DownArrow))) {
				//Slide
				StartSliding();
				Invoke("StopSliding", 1.0f); 
			}


			if (Input.GetKeyDown(KeyCode.Space)) {
				//Jump
				anim.SetTrigger("Jump");
				verticalVelocity = jumpForce;
			}
            */
        }
        else
        {
            //falling
            verticalVelocity -= (gravity * Time.deltaTime);
            /*
			//fast falling mechanic
			if (MobileInput.Instance.SwipeDown) {
				verticalVelocity = -jumpForce;
			}

			//fast falling mechanic
			if(Input.GetKeyDown(KeyCode.Space)){
				verticalVelocity = -jumpForce;
			}
            */

        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // move the Pengu
        controller.Move(moveVector * Time.deltaTime);

        // Rotate the Pengu to where he is going
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }


    private bool IsGrounded()
    {
        /*
		 *  LEARN THIS AGAIN LESSON 3 9:00 MAYBE
		 */


        // ray is two parameters object that have orginal position and direction
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x,
            (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
            controller.bounds.center.z),
            Vector3.down);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);

    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);

    }

    public void StartRunning()
    {
        //  isRunning = true;
        this.anim.SetTrigger("StartRuning");

    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);

    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }

    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        // isRunning = false;

    }

    //ON TRIGGER
    private void OnTriggerEnter(Collider other)
    {

        switch (other.tag)
        {
            case "waypoint":

                UpdateNewTarget();
                break;
            case "waypoint-slide":
                //play slide animation
               // StartSliding();
                Invoke("StopSliding", 1.0f);
                UpdateNewTarget();
                break;
            case "waypoint-jump":
                //play jump animation
                anim.SetTrigger("Jump");
              //  verticalVelocity = jumpForce;
                UpdateNewTarget();
                break;
        }

    }


    private void UpdateNewTarget()
    {
        // update the new target is the LevelManager script
        GameObject.Find("LevelManager").GetComponent<LevelManager>().UpdateM_HasTarget();

    }


    public void setDestination( Transform m_wayPointTranform)
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        //  _navMeshAgent.destination = GameObject.Find("LevelManager").GetComponent<LevelManager>().m_wayPoint;
      //  _destination = m_wayPointTranform;
            _navMeshAgent.destination = m_wayPointTranform.position;
           // _navMeshAgent.SetDestination(_destination.position);
            Debug.Log(_navMeshAgent.destination);
	}
}
 