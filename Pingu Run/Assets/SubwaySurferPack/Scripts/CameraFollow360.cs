using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow360 : MonoBehaviour {

	static public Transform player;
	public float distance = 15;
	public float height = 10;
	private Vector3 lookOffset = new Vector3(0,3,-3);
	public float cameraSpeed = 1000;
	public float rotSpeed = 1000;
    public Vector3 rotation = new Vector3(35, 0, 0);

    void FixedUpdate () 
	{
       
		if(player)
		{
			//Vector3 lookPosition = player.position + lookOffset;
			//Vector3 relativePos = lookPosition - transform.position;
   //     	Quaternion rot = Quaternion.LookRotation(relativePos);
			
			//transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * rotSpeed * 1.1f);

   //         Vector3 targetPos = player.transform.position + player.transform.up * height - player.transform.forward * distance;
   //         targetPos.x = 0;
			
			//this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * cameraSpeed * 1.1f);


            Vector3 desirePosition = player.position + lookOffset;
            desirePosition.x = 0;

            transform.position = Vector3.Lerp(transform.position, desirePosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
        }
	}
}


