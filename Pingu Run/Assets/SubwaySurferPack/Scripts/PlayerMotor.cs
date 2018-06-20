using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCEC = 2.5f;
    private const float TURN_SPEED = 0.05f;

    public bool isAIGame = false;

    public bool isRunning = false;

    //Animation
    private Animator anim;


    //Movement
    private float jumpForce = 6f;
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


    public void setAigame()
    {

        isAIGame = true;
    }



    private void Start()
    {
        speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {

        // !isRunning so not do anything;
        if (!isRunning)
        {
            return;
        }

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            //Change the Modifier text
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        //Gather the inputs on which we should be
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);
        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);


        // gather the input on which lane we should be
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Move Left
            MoveLane(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Move Right
            MoveLane(true);
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;

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


        /*
	 * JUMP
	 */

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);
        //calaulate Y
        if (isGrounded)
        { //if grounded
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp || (Input.GetKeyDown(KeyCode.UpArrow)))
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }

            if (MobileInput.Instance.SwipeDown || (Input.GetKeyDown(KeyCode.DownArrow)))
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }

        }
        else
        {
            //falling
            verticalVelocity -= (gravity * Time.deltaTime);

            //fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }

            //fast falling mechanic
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                verticalVelocity = -jumpForce;
            }

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
        isRunning = true;
        anim.SetTrigger("StartRuning");

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

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }


    public void CrashAI()
    {
        //desiredLane = 1;
        speed = originalSpeed;

        anim.SetTrigger("Death");

        isRunning = false;
        Invoke("RestartRunAfterCol", 2f);

    }

    void RestartRunAfterCol()
    {
        isRunning = true;
        //n_anim.SetTrigger("Death");
        anim.SetTrigger("Jump");
        anim.SetTrigger("StartRuning");

    }

    private IEnumerator OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isAIGame)
        {

            switch (hit.gameObject.tag)
            {
                case "Obstacle":
                    //Destroy(hit.transform.parent.gameObject);
                    hit.transform.parent.gameObject.SetActive(false);
                    CrashAI();
                    yield return new WaitForSeconds(3);
                    hit.transform.parent.gameObject.SetActive(true);
                    break;

                case "SnowBall":
                    Destroy(hit.gameObject);
                    CrashAI();
                    break;
            }

        }

        else
        {
            switch (hit.gameObject.tag)
            {
                case "Obstacle":
                    Crash();
                    break;

            }
        }
    }

}
