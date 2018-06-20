using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameAi : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndGame(true); 
        }
        if (collider.gameObject.tag == "AI")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndGame(false);
        }
    }

}
