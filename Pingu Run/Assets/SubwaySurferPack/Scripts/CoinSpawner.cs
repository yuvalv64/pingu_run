
using UnityEngine;

public class CoinSpawner : MonoBehaviour {
	// by deafult the max Coins is 5 and that also in the prefab
	public int maxCoin =5;
	public float chanceToSpawn = 0.5f;
	public bool forceSpawnAll = false;


	private GameObject[] coins;

	// awake lunched befora the object is active
	private void Awake(){
		// the size of the array is the amount of the childs = transform.childCount
		coins = new GameObject[transform.childCount];
		for(int i =0; i < transform.childCount ; i++){
			//initial the array with the childs
			coins [i] = transform.GetChild (i).gameObject;
		}
		OnDisable ();
	}
	private void OnEnable(){
		if (Random.Range (0.0f, 1.0f) > chanceToSpawn) {
			// the spawn is failed
			return;
		} 
		if (forceSpawnAll) {
			for(int i =0; i < maxCoin ; i++){
				coins [i].SetActive (true);
			}
				
		} else {
			// r = how many coin are we spawning
			int r = Random.Range (0,maxCoin);
			for(int i =0; i < r ; i++){
				coins [i].SetActive (true);
			}
		}
	}

	private void OnDisable(){
		foreach (GameObject go in coins)
			go.SetActive (false);
	}
}
