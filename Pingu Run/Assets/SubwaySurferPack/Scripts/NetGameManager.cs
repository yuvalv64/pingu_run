using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class NetGameManager : NetworkBehaviour
{


	private const int COIN_SCORE_AMOUNT = 5;
    //public static GameManager Instance {set; get;}
    public static NetGameManager singleton;

    public bool strGame = false;


	public bool isDead { set; get;}
	private bool isGameStarted = false;
    public GameObject[] players;
    public GameObject localPlayer;
    // private PlayerAi motorAI;

    //Death Menu
    public Animator deathMenuAnim;
	public Text deadScoreText, deadCoinText;

	//UI and the UI fields
	public Animator gameCanvas, menuAnim , diamondAnim;
	public Text scoreText, coinText, modifierText , hiScore;
    public float score, coinScore, modifierScore=0.0f;
	private int lastScore;

    public GameObject winnerUi;
    public GameObject loserUi;
    public Text place;
    public GameObject icon;

    public void Awake()
    {
        singleton = this;
    }

    public void Fire()
    {
        icon.SetActive(false);
    }

    public void reload()
    {
        icon.SetActive(true);
    }


    private int get_place()
    {
        int pos = 1;

        Transform p = localPlayer.transform;



        for(int i = 0; i < players.Length; i++)
        {

            if (players[i].transform.position.z > p.position.z)
                pos++;

        }
    
        return pos;
    }


    private void Start()
	{
        // initlize text, motor , socres
        //Instance = this;
		modifierScore = 1;
       
        modifierText.text ="x" + modifierScore.ToString ("0.0");
		coinText.text = coinScore.ToString("0");
		scoreText.text = scoreText.text= score.ToString ("0");
		hiScore.text = PlayerPrefs.GetInt ("Hiscore").ToString();
        AddPlayer();

    }

    public void StartGame()
    {

        isGameStarted = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        //FindObjectOfType<cameraMotor>().IsMoving = true;
        gameCanvas.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");

    }



    public void AddPlayer()
    {

        players = GameObject.FindGameObjectsWithTag("NotLocalPlayer");
        localPlayer = GameObject.FindGameObjectWithTag("Player");
    }


    private void FixedUpdate()
	{


		if (isGameStarted && !isDead) {
			//Update the score up
			score += (Time.deltaTime * modifierScore);
			if(lastScore!=(int)score){
				lastScore = (int)score;
				scoreText.text = score.ToString ("0"); // because problams with the cpu we don't call UpdateScores function from here
			}

		}

        int p = get_place();
        place.text = "" + p;


    }


    public void GetCoin(){
		coinScore++;
		coinText.text = coinScore.ToString ("0");
		score+= COIN_SCORE_AMOUNT;
		scoreText.text= score.ToString ("0");
		diamondAnim.SetTrigger ("Collect");
	}
		

	public void UpdateModifier(float modifierAmount){
        // update the score
		modifierScore = 1.0f + modifierAmount;
		modifierText.text ="x" + modifierScore.ToString ("0.0");
	}


    // start the game by laod the "GAME" scene
	//public void OnPlayButton(){
	//	UnityEngine.SceneManagement.SceneManager.LoadScene ("Game");
	//}

	public void OnDeath(){
		isDead= true;
		FindObjectOfType<GlacierSpawner> ().IsScrolling = false;
		deadScoreText.text = score.ToString ("0");
		deadCoinText.text = coinScore.ToString ("0");
		deathMenuAnim.SetTrigger ("Dead");
		gameCanvas.SetTrigger ("Hide");

		//Check if this is a highscore
		if(score >PlayerPrefs.GetInt("Hiscore")){
			float s = score;
			if (s % 1 == 0) {
				s += 1;
			}

			PlayerPrefs.SetInt ("Hiscore",(int)s);
		}
	}
}
