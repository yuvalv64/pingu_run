using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerAi : MonoBehaviour {

	private Transform playerTransform;

	// Use this for initialization
	void Start () {
		// Tranform object have a Position, rotatio and scale of an object.
		// GameObject is a base class for all entities in the unity scenes.

		playerTransform = GameObject.FindGameObjectWithTag ("AI").transform;
	}
	
	// Update is called once per frame
	void Update () {
		// only in the Z and noy y,x
		transform.position = Vector3.forward * (playerTransform.position.z);

	}
}
