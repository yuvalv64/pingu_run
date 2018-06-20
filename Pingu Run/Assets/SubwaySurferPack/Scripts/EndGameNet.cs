using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameNet : MonoBehaviour {

    NetPlayerMotor nt;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            nt = collider.GetComponent<NetPlayerMotor>();
            nt.endGame();
        }
    }

}
