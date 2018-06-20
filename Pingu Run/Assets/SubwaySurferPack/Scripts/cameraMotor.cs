using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMotor : MonoBehaviour {

	public Transform lookAt; //our pengu
	public Vector3 offset=new Vector3(0,5.0f,-10.0f);
	public bool IsMoving {set; get;}
	public Vector3 rotation = new Vector3 (35, 0, 0);
		
	private void LateUpdate(){
		if (!IsMoving)
			return;
		//it's a late because the player move first and after it the camera
		Vector3 desirePosition = lookAt.position+offset;
		desirePosition.x = 0; 

		transform.position = Vector3.Lerp (transform.position,desirePosition,0.1f);
		transform.rotation = Quaternion.Lerp (transform.rotation,Quaternion.Euler(rotation),0.1f);
	}
}
