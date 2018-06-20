using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    public GameObject place;

    public void PlayGameEndlessRun()
    {
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().gameType = "EndlessRunGame";
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().setUpGameModeProperties();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().updateAfterClick();
        place.SetActive(false);
        
    }

    public void PlayMultiplayerGame()
    {
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().gameType = "MultiplayerGame";
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().setUpGameModeProperties();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().updateAfterClick();
        SceneManager.LoadScene("loby");
        
    }


    public void PlayGameAI()
    {
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().gameType = "AiGame";
        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().setUpGameModeProperties();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().updateAfterClick();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>().setAigame();
        place.SetActive(true);
    }




    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
