using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour {

	private const float DEADZONE = 100.0f;
	public static MobileInput Instance { set; get; }

	private bool tap, swipeLeft , swipeRight, swipeUp, swipeDown;
	/*startTouch - keep in memory where the user start the drag
	 * swipeDelta - the current position by the drag the distance between the startTouch and the swipeDelta
	 */
	private Vector2 swipeDelta , startTouch;
	/*
	 *  GET ?!?!?!?!?!?!?!?
	 */
	public bool Tap{ get { return tap; } }

	public Vector2 SwipeDelta{ get { return swipeDelta; } }
	public bool SwipeLeft{ get { return swipeLeft; } }
	public bool SwipeRight{ get { return swipeRight; } }
	public bool SwipeUp{ get { return swipeUp; } }
	public bool SwipeDown{ get { return swipeDown; } }

	private void Awake(){
		Instance = this;
	}

	private void Update(){
		//Reseting all the booleans evry frame - each frame the bools will get true/false 
		tap=swipeLeft=swipeRight=swipeDown=swipeUp=false;

		//let's check for Input

		#region Standalone Inputs
		if(Input.GetMouseButtonDown(0)){
			// 0 = left click
			tap=true;
			// current start position touch
			startTouch = Input.mousePosition;
		}

		else if(Input.GetMouseButtonUp(0)){
			startTouch = swipeDelta=Vector2.zero;
		}
		#endregion

		#region Mobile Inputs
		if(Input.touches.Length!=0){
			if(Input.touches[0].phase ==TouchPhase.Began){
				tap=true;
				// current start position touch
				startTouch = Input.mousePosition;
			}	else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase ==TouchPhase.Canceled){
				startTouch = swipeDelta=Vector2.zero;
			}

		}
		else if(Input.GetMouseButtonUp(0)){
			startTouch = swipeDelta=Vector2.zero;
		}
		#endregion

		//Calculate distance (the swipeDelta) between the startTouch and the current position
		swipeDelta = Vector2.zero;
		if(startTouch!=Vector2.zero){
			
			//startTouch!=Vector2.zero it's mean that we actually start touching somewhere ,let's check with mobile
			if (Input.touches.Length != 0) {
				swipeDelta = Input.touches [0].position - startTouch;
			} 
			//let's check with standalone
			else if (Input.GetMouseButton(0)) {
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
			}

			//Check if were beyond the DEADZONE

			if(swipeDelta.magnitude >DEADZONE){
				// 100 pixels

				//this is a confirmed swipe - cehck the diraction of the swipe
				float x = swipeDelta.x;
				float y = swipeDelta.y;

				// Mathf.Abs to make the number to absuloute - number - > |number|
				if (Mathf.Abs (x) > Mathf.Abs (y)) {
					//Left or Right
					if (x < 0)
						swipeLeft = true;
					else
						swipeRight = true;
				} else {
					//UP to Down
					if (y < 0)
						swipeDown = true;
					else
						swipeUp = true;
				}

				startTouch = swipeDelta = Vector2.zero;
			}
		}
	}
}
