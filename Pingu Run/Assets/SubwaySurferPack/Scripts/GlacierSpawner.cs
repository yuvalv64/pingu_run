using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierSpawner : MonoBehaviour {

	private const float DISTANCE_TO_RESPAWN=10.0f;
	public float scrollSpeed = -2f;
	public float totalLength;
	public bool IsScrolling{ set; get; }

	private float scrollLocation;
	private Transform playerTranform;


	private  void Start(){
		playerTranform = GameObject.FindGameObjectWithTag ("Player").transform;

	}

	private void Update(){
		
		if (!IsScrolling) {
			return;
		} else {
			
			scrollLocation += scrollSpeed * Time.deltaTime;
			Vector3 newLocation = (playerTranform.position.z + scrollLocation) * Vector3.forward;
			transform.position = newLocation;

			if(transform.GetChild(0).transform.position.z < playerTranform.position.z -DISTANCE_TO_RESPAWN){
				transform.GetChild (0).localPosition += Vector3.forward * totalLength;
				// Push it back into the list - as last object
				transform.GetChild (0).SetSiblingIndex (transform.childCount);

				transform.GetChild (0).localPosition += Vector3.forward * totalLength;
				// Push it back into the list
				transform.GetChild (0).SetSiblingIndex (transform.childCount);
			}
		}
	}
}
