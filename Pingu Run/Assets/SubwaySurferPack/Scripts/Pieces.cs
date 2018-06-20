using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enum function Declare a number of values that can be obtained
public enum PieceType{

	none =-1,
	ramp=0,
	longblock =1,
	jump = 2,
	slide = 3,
}

public class Pieces : MonoBehaviour {

	public PieceType type;
	public int visualIndex;

}
