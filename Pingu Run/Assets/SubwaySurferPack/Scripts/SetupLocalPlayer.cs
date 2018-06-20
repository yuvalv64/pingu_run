using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupLocalPlayer : NetworkBehaviour {

    Animator anim;
    NetworkAnimator n_anim;


    void Awake()
    {
        
        anim = GetComponentInChildren<Animator>();
        n_anim = GetComponentInChildren<NetworkAnimator>();
        anim.SetBool("Grounded", true);

        //Invoke("StartGame", 3.0f);
    }

    public void StartGame()
    {
        NetPlayerMotor s = GetComponent<NetPlayerMotor>();
        s.StartRunning();

        NetGameManager.singleton.StartGame();
        NetGameManager.singleton.strGame = true;

    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        gameObject.name = "LocalNetPlayer";
    }




    [SyncVar(hook = "OnChangeAnimation")]
   public string animState = "Grounded";

   void OnChangeAnimation(string aS)
   {
       //if (isLocalPlayer) return;
       UpdateAnimationState(aS);
   }

   [Command]
   public void CmdChangeAnimState(string aS)
   {
       UpdateAnimationState(aS);
   }

   void UpdateAnimationState(string aS)
   {
       if (animState == aS) return;
       animState = aS;
       Debug.Log(animState);
       if (animState == "Grounded")
           anim.SetBool("Grounded", true);
       else if (animState == "StartRuning")
           n_anim.SetTrigger("StartRuning");
       else if (animState == "Sliding")
           anim.SetBool("Sliding", true);
       else if (animState == "StopSliding")
           anim.SetBool("Sliding", false);
       else if (animState == "Jump")
           n_anim.SetTrigger("Jump");
   }


    // Use this for initialization
    void Start () 
	{

        if (isLocalPlayer)
		{
            GetComponent<NetPlayerMotor>().set = this;
            CameraFollow360.player = this.gameObject.transform;
            
        }
        if (!isLocalPlayer)
            transform.tag = "NotLocalPlayer";

    }

	void Update()
	{
		//determine if the object is inside the camera's viewing volume
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && 
		                screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

}
