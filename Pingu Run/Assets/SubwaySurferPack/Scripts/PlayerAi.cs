using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAi : MonoBehaviour
{
    private const float LANE_DISTANCEC = 2.5f;
    private const float TURN_SPEED = 0.05f;

    //Animation
    private Animator anim;

    NavMeshAgent _navMeshAgent;
    //Movement
    private float jumpForce = 4f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1; //0= Left , 1= Middle, 2= Right
    private CharacterController controller;

    //Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    public Vector3 temporaryTarget;
    public GameObject levelM;
    public LevelManager levelScript;

    public Transform playerTransform;

    public FireContAi fireContAi;

    private void Awake()
    {

       
        levelM = GameObject.Find("LevelManager");
        levelScript = levelM.GetComponent<LevelManager>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.isStopped = true;
        _navMeshAgent.speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        fireContAi = GetComponent<FireContAi>();
        anim = GetComponent<Animator>();
      
    }

    private void Update()
    {

        // get the target every time
        temporaryTarget = levelScript.m_wayPoint;


        // !isRunning so not do anything;

        if (!(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>().isRunning))
        {
            return;
        }
        _navMeshAgent.isStopped = false;
        StartRunning();

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            _navMeshAgent.speed += speedIncreaseAmount;
            //Change the Modifier text
            GameManager.Instance.UpdateModifier(_navMeshAgent.speed - originalSpeed);
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

        //shot
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Transform transformV = GetComponent<Transform>();

        

        if ( Math.Abs(playerTransform.position.x - transformV.position.x) <= 1)
        {

            if (Math.Abs(playerTransform.position.y - transformV.position.y) <= 1)
            {
                if ((playerTransform.position.z - transformV.position.z) > 2)
                {

                    fireContAi.tryShot();
                }
            }
        }






        /*
	 * JUMP
	 */

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

        // NOT NEEDED
          _navMeshAgent.Move(moveVector * Time.deltaTime);

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
       
        anim.SetTrigger("StartRuning");

    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        _navMeshAgent.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
        
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    public void Crash()
    {
        //desiredLane = 1;
        speed = originalSpeed;

        anim.SetTrigger("Death");

        Invoke("RestartRunAfterCol", 2f);

    }

    void RestartRunAfterCol()
    {
        //n_anim.SetTrigger("Death");
        anim.SetTrigger("Jump");
        anim.SetTrigger("StartRuning");

    }

    //ON COLLISION
    private IEnumerator OnControllerColliderHit(ControllerColliderHit hit)
    {

        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                //Destroy(hit.transform.parent.gameObject);
                hit.transform.parent.gameObject.SetActive(false);
                Crash();
                yield return new WaitForSeconds(3);
                hit.transform.parent.gameObject.SetActive(true);
                break;

            case "SnowBall":
                Destroy(hit.gameObject);
                Crash();
                break;
        }

    }
    //ON TRIGGER
    private void OnTriggerEnter(Collider other)
    {
      
        switch (other.tag)
        {
            case "waypoint":
                levelScript.UpdateM_HasTarget();
                break;
            case "waypoint-slide":
                //play slide animation
                StartSliding();
                Invoke("StopSliding", 1.0f);
                 levelScript.UpdateM_HasTarget();
                break;
            case "waypoint-jump":
                //play jump animation
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
                levelScript.UpdateM_HasTarget();
                break;
            case "Obstacle":
                Crash();
                break;
        }

    }

        public void setDestination(Transform m_wayPointTranform)
    {
        _navMeshAgent.SetDestination(m_wayPointTranform.position);

    }

    public void NewTarget()
    {
        //get the new target from the LevelMnagaer Script
        temporaryTarget = levelScript.m_wayPoint;
    }
}
