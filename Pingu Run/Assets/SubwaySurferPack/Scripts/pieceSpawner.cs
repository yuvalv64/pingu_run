using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceSpawner : MonoBehaviour {

	public PieceType type;
	private Pieces currentPiece;

	public void Spawn(){
		int amtObj = 0;
		switch (type) {
		case PieceType.jump:
			amtObj = LevelManager.Instance.jumps.Count;
			break;
		case PieceType.slide:
			amtObj = LevelManager.Instance.slides.Count;
			break;
		case PieceType.longblock:
			amtObj = LevelManager.Instance.longblocks.Count;
			break;
		case PieceType.ramp:
			amtObj = LevelManager.Instance.ramps.Count;
			break;
		}
		currentPiece = LevelManager.Instance.GetPiece (type, Random.Range(0,amtObj));
		// get randomally a piece from the pool
		currentPiece.gameObject.SetActive (true);
		currentPiece.transform.SetParent (transform,false);
	}
	public void DeSpawn(){
		currentPiece.gameObject.SetActive (false); // and go back to the pool
	}
}
