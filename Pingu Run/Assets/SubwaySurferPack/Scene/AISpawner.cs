using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//show in the inspector
[System.Serializable]
public class AIObjects
{
    // declare our vars
    public string AIGroupName { get { return m_aiGroupName; } }
    public GameObject objectPrefab { get { return m_prefab; } }
    public int maxAi { get { return m_maxAI; } } // max number of ai that we agree in our group
    public int spawnRate { get { return m_spawnRate; } } // the frequent of the ai respawned ??
    public int spawnAmount { get { return m_maxSpawnAmount; } } // max spawn amount of the ais ??
    public bool randomizeStats { get { return m_randomizeStats; } }
    // AIGroupName is a fake var that give us the value of m_aiGroupName


    public bool enableSpawner { get { return m_enableSpawner; } }



    //serliaze the private variables
    // header for the inspector
    [Header("AI Group Stats")]
    [SerializeField]
    private string m_aiGroupName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Range(0f, 40f)]
    private int m_maxAI;
    [SerializeField]
    [Range(0f, 20f)]
    private int m_spawnRate;
    [SerializeField]
    [Range(0f, 10f)]
    private int m_maxSpawnAmount;

    [Header("Main setting")]
    [SerializeField]
    private bool m_enableSpawner;
    [SerializeField]
    private bool m_randomizeStats;

    public AIObjects(string Name , GameObject Prefab , int MaxAI, int SpawnRate, int SpawnAmount, bool RandomizeStats)
    {
        this.m_aiGroupName = Name;
        this.m_prefab = Prefab;
        this.m_maxAI = MaxAI;
        this.m_spawnRate = SpawnRate;
        this.m_maxSpawnAmount = SpawnAmount;
        this.m_randomizeStats = RandomizeStats;
    }
    public void setValues(int MaxAI, int SpawnRate, int SpawnAmount)
    {
        this.m_maxAI = MaxAI;
        this.m_spawnRate = SpawnRate;
        this.m_maxSpawnAmount = SpawnAmount;
    }

}



public class AISpawner : MonoBehaviour {

    // ---------->
    //variables

    public List<Transform> wayPoints = new List<Transform>();

    public float spawnTimer { get { return m_SpawnTimer; } }
    public Vector3 spawnArea { get { return m_SpawnArea; } }


    [Header("Gloabl Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float m_SpawnTimer;
    [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f);
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(20f, 10f, 20f);



    //create array from new class
    [Header("AI Group Setting")]
    public AIObjects[] AIObject = new AIObjects[5];


    // Use this for initialization
    void Start () {
        GetWayPoint();
        RandimizeGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer);
    }
	
	// Update is called once per frame
	void Update () {
   

    }

    void spawnNPC()
    {
        for (int i=0; i<AIObject.Count();i++)
        {
            if (AIObject[i].enableSpawner && AIObject[i].objectPrefab !=null)
            {

                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if (tempGroup.GetComponentInChildren<Transform>().childCount <AIObject[i].maxAi) {
                    for (int y = 0; y < Random.Range(0,AIObject[i].spawnAmount); y++)
                    {
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(360, 0), 0);

                        // create spawnd gameObject
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].objectPrefab, RandomPosition(), randomRotation);
                        //put spawned NPC as child of group
                        tempSpawn.transform.parent = tempGroup.transform;
                        // add the AIMove to the NPC object
                        tempSpawn.AddComponent<AIMove>(); // tempSpawn.AddComponent<AIMove>() -  we need to add it to the pingu PL

                    }



                }

            }
        }
    }

    public Vector3 RandomWayPoint()
    {
        int randomWP = Random.Range(0,(wayPoints.Count-1));
        Vector3 randomWypoint = wayPoints[randomWP].transform.position;
        return randomWypoint;
    }

    public Vector3 RandomPosition()
    {
        Vector3 randomPosition = new Vector3
       (
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
       );
        randomPosition = transform.TransformPoint(randomPosition * 0.5f);
        return randomPosition;
    }

    //method for putting random values in the AI Group Setting
    void RandimizeGroups()
    {
        //Randomise

        for (int i=0; i<AIObject.Count(); i++)
        {

            if (AIObject[i].randomizeStats)
            {

                //  AIObject[i]= new AIObjects(AIObject[i].AIGroupName, AIObject[i].objectPrefab, Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10), AIObject[i].randomizeStats);
                AIObject[i].setValues(Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10));

            }

        }

    }

    //Method for creating the empty worldobject groups
    void CreateAIGroups()
    {

        for (int i=0; i<AIObject.Count(); i++)
        {

          //empty object to keep our ai in
             GameObject m_AIGroupSpawn;


         //create a new gameObject
          m_AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
            m_AIGroupSpawn.transform.parent = this.gameObject.transform;
        }

    }


    void GetWayPoint()
    {

        Transform[] wpList = this.transform.GetComponentsInChildren<Transform>();

        for(int i=0; i<wpList.Length; i++)
        {
            if(wpList[i].tag == "waypoint")
            {
                wayPoints.Add(wpList[i]);
            }
        }

    }

    // show the gizmos in color
    void OnDrawGizmosSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position,spawnArea);
    }

}
