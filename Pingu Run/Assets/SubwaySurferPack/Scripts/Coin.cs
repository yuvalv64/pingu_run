using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
	private Animator anim;

	private void Awake(){
		anim = GetComponent<Animator> ();
	}

	private void OnEnable(){
		anim.SetTrigger ("Spawn");
	}

	private void OnTriggerEnter(Collider other){
		if (other.tag == "Player" || other.tag == "AI") {
			GameManager.Instance.GetCoin ();
            Invoke("SetAnimCollected", 0.2f);
           
        }
	}

    private void SetAnimCollected()
    {
        anim.SetTrigger("Collected");
    }
}
