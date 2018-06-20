using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndNetGame : MonoBehaviour {

    public void LoadLobyScene()
    {
        SceneManager.LoadScene("Game");
    }
}
