using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{

    public int SegId { set; get; }
    public bool transition;

    public int length;
    // the start and end of each segment in each lane.
    public int beginY1, beginY2, beginY3;
    public int endY1, endY2, endY3;

    private pieceSpawner[] piecesArr;


    // Awake is called when the script instance is being loaded;
    private void Awake()
    {
        piecesArr = gameObject.GetComponentsInChildren<pieceSpawner>();
        //enable the mesh collider only if the SHOW_COLLIDER is on. - that for the collider and the death collider.
        for (int i = 0; i < piecesArr.Length; i++)
        { //
            foreach (MeshRenderer mr in piecesArr[i].GetComponentsInChildren<MeshRenderer>())
                mr.enabled = LevelManager.Instance.SHOW_COLLIDER;
        }
    }


    //function to prevent from overLoad our memory
    public void Spawn()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < piecesArr.Length; i++)
        {
            piecesArr[i].Spawn();
        }

    }

    public void DeSpawn()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < piecesArr.Length; i++)
        {
            piecesArr[i].DeSpawn();
        }
    }


}
