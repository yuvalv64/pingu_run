using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{

    public bool SHOW_COLLIDER = true; //$$

    public static LevelManager Instance { set; get; }

    public bool gameModeChoosed;
    public string gameType;
    public GameObject startPositionDistance;

    //Level Spawing
    // how meters look forward before a spawn is carried out
    private const float DISTANCE_BEFORE_SPAWN = 150.0f;
    //how many segment we want when the game start
    private const int INITIAL_SEGMENT = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    //what is the max of segment each time on the screen
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    //will say to us where we are actually on the screen right now.
    private Transform cameraContainer;
    //the number of active segment rights now
    private int amountOfActiveSegments;
    //when we want to make a break *****
    private int continiousSegments;
    //current position
    private int currentSpawnZ;
    //we are on ground or on top of a train maybe
    private int currentLevel;

    //tell if the each leans on ground or not
    private int y1, y2, y3;


    // List of pieces

    public List<Pieces> ramps = new List<Pieces>();
    public List<Pieces> longblocks = new List<Pieces>();
    public List<Pieces> jumps = new List<Pieces>();
    public List<Pieces> slides = new List<Pieces>();
    [HideInInspector]
    public List<Pieces> pieces = new List<Pieces>(); // All the active pieces in the pool

    //List of Segments:
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    
    public List<Segment> Segments = new List<Segment>(); // the active segments


    //Gameplay
    private bool IsMoving = false;

    // ------------ AI
    public List<Transform> wayPoints = new List<Transform>();
    public bool m_hasTarget = false; // have we lock down on target?
    public Vector3 m_wayPoint; // for defer between the two waypoints
    private Vector3 m_lastWaypoint = new Vector3(0f, 0f, 0f);
    System.Random rnd = new System.Random();
    float gCost, hCost, fCost;

    //Awake is used to initialize any variables or game state before the game starts.
    private void Awake()
    {
        // Instance = this - create an instance of the object that the script is attach to
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
        gameType = "AiGame";
        gameModeChoosed = false;
    }

    // the Start function called maybe a one frame ofter the awake finish here Job
    private void Start()
    {
        // first we create a space between the player and the first obstcales
        for (int i = 0; i < INITIAL_SEGMENT; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
                SpawnTransition();
            else
                GenerateSegment();
        }
        
        if (gameType.Equals("AiGame"))
        {
            // initilaize the first target for the AI
            m_wayPoint = wayPoints[0].transform.position;
            GameObject.FindGameObjectWithTag("AI").GetComponent<PlayerAi>().setDestination(wayPoints[0].transform);
         
        }
       
      
       

    }

    public void setUpGameModeProperties()
    {
        if (gameType.Equals("EndlessRunGame") || gameType.Equals("MultiplayerGame"))
        {
            GameObject.FindGameObjectWithTag("AI").SetActive(false);
            GameObject.FindGameObjectWithTag("Plane").SetActive(false);
            GameObject.FindGameObjectWithTag("FinishLine").SetActive(false);

        }
        if (gameType.Equals("AiGame"))
        {
            GameObject.FindGameObjectWithTag("AI").SetActive(true);
            GameObject.FindGameObjectWithTag("Plane").SetActive(true);
        }
    }

    private void Update()
    {

        // spawn another segment - and check it each frame - this is whay we do it in the update function
        if (currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
        {
            GenerateSegment();
        }


        // make Despawn
        if (amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            Segments[amountOfActiveSegments - 1].DeSpawn();
            // set the order of tge segments as well
            Segments.Remove(Segments[amountOfActiveSegments - 1]);
            amountOfActiveSegments--;
        }

        if (gameType.Equals("AiGame"))
        {
            // change target if the PlayerAi is after the current target
            if (GameObject.FindGameObjectWithTag("AI").transform.position.z > wayPoints[0].transform.position.z)
            {
                UpdateM_HasTarget();
            }
        }

    }

    bool CanFindTarget()
    {
        //get the first target each time
        m_wayPoint = wayPoints[0].transform.position;
        GameObject.FindGameObjectWithTag("AI").GetComponent<PlayerAi>().setDestination(wayPoints[0].transform);
       
        //make sure we not set the same waypoint twice
        if (m_lastWaypoint == m_wayPoint)
        {
            // to the next waypoint
            m_wayPoint = wayPoints[1].transform.position;
            return false;
        }
        else
        {
            // set the last way point to be the new one
            m_lastWaypoint = m_wayPoint;

            // change the animator speed for the Defeqult Level - TIP


            return true;
        }
    }


    public void UpdateM_HasTarget()
    {
        
        // the AI player reach the target so m_hasTarget is true and update the new target
        m_hasTarget = true;
        // change to the next target in the array
        changeTarget();
        
        // update the temporaryTarget in the PlayerAi script
       
    }

    void changeTarget()
    {

        // change to if we reach the target
        if (m_hasTarget == true)
        {
            // remove the current target
            
            wayPoints.Remove(wayPoints[0].transform);

            //update the current target to be the next target
            m_wayPoint = wayPoints[0].transform.position;
            
            GameObject.FindGameObjectWithTag("AI").GetComponent<PlayerAi>().setDestination(wayPoints[0].transform);
            // m_hasTarget = false because the AI isn't reach the new target
            m_hasTarget = false;
        }
    }

    /*public void GetWayPointSegment (Segment s){
        // take all the children of any segment that was spawned
        Transform[] wpList = s.transform.GetComponentsInChildren<Transform>();
        // GameObject[] wpList = GetComponentsInChildren<GameObject>();
        List<Transform> tranformTargets = new List<Transform>();

        for (int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "waypoint" || wpList[i].tag == "waypoint-slide" || wpList[i].tag == "waypoint-jump")
            {
                tranformTargets.Add(wpList[i]);
            }

        }

        
        int numToSetActive = rnd.Next(0, tranformTargets.Count());

        for (int i = 0; i < tranformTargets.Count(); i++)
        {
           tranformTargets[i].gameObject.SetActive(false);
        }

        tranformTargets[numToSetActive].gameObject.SetActive(true);
       
        wayPoints.Add(tranformTargets[numToSetActive]);

        //clear the list
        tranformTargets.Clear();

    }
    */

    public void GetWayPointSegment(Segment s)
    {
        // take all the children of any segment that was spawned
        Transform[] wpList = s.transform.GetComponentsInChildren<Transform>();
        // GameObject[] wpList = GetComponentsInChildren<GameObject>();
        List<Transform> tranformTargets = new List<Transform>();


        for (int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "waypoint" || wpList[i].tag == "waypoint-slide" || wpList[i].tag == "waypoint-jump")
            {
                tranformTargets.Add(wpList[i]);
            }

        }

        //A* f(n) = g(n) +h(n)
        gCost = Vector3.Distance(transform.position, startPositionDistance.transform.position);
        float minimalHcost = Vector3.Distance(tranformTargets[0].position, transform.position);
        int counterTarget = 0;

        for (int i = 0; i < tranformTargets.Count; i++)
        {
            if (Vector3.Distance(tranformTargets[i].position, transform.position) + gCost < minimalHcost + gCost)
            {
                minimalHcost = Vector3.Distance(tranformTargets[i].position, transform.position);
                counterTarget = i;
            }
        }

        //   int numToSetActive = rnd.Next(0, tranformTargets.Count());

        for (int i = 0; i < tranformTargets.Count(); i++)
        {
            if (i != counterTarget)
            {
                tranformTargets[i].gameObject.SetActive(false);
            }

        }

        // tranformTargets[numToSetActive].gameObject.SetActive(true);

        wayPoints.Add(tranformTargets[counterTarget]);

        //clear the list
        tranformTargets.Clear();

    }

    //public void GetWayPoint(Segment s)
    //{
    //    // take all the children of any segment that was spawned

    //    Transform[] wpList = s.transform.GetComponentsInChildren<Transform>();
    //    // GameObject[] wpList = GetComponentsInChildren<GameObject>();

    //   for (int i = 0; i < wpList.Length; i++)
    //   {
    //       // the AI targets colliders
    //       if (wpList[i].tag == "waypoint" || wpList[i].tag == "waypoint-slide" || wpList[i].tag == "waypoint-jump")
          
    //       {
    //           /*
    // REMEMBER TO ADD TARGET TO THE TRANSITION SEGMENTS
    
    
    
    //*/
    //           // add a new target from segment that was spawned at the end list of targets
    //           wayPoints.Add(wpList[i]);
    //       }

    //   }

    //}

    public void GenerateSegment()
    {

        SpawnSegment();

        if (Random.Range(0f, 1f) < (continiousSegments * 0.25f))
        {
            //time for Spawn transition seg
            continiousSegments = 0;
            SpawnTransition();

        }
        else
        {

            continiousSegments++;
        }
    }

    private void SpawnSegment()
    {
        List <Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);

        // get the random segment from the pool
        int id = Random.Range(0, possibleSeg.Count);
      
        Segment s = GetSegment(id, false);

        // get the targets for the AI from each segement object

        if(gameType.Equals("AiGame"))
             GetWayPointSegment(s);

        // when the segment finish to spawning
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTranstion = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTranstion.Count);
        
        Segment s = GetSegment(id, true);
        if (gameType.Equals("AiGame"))
            GetWayPointSegment(s);
        // when the segment finish to spawning
        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();

    }

    public Segment GetSegment(int id, bool transition)
    {

        // the Segments is the public List<Segment> Segments = new List<Segment>(); that we created up
        Segment s = null;
        s = Segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

        if (s == null)
        {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();
            s.SegId = id;
            s.transition = transition;

            Segments.Insert(0, s);
        }
        else
        {

            Segments.Remove(s);

            // we removed that segment from the pool after they spawned and add him again but now it will be at the first index

            //the insert function is diffrent from the add( line 134 for now) function - because you have the option to choose we
            // to put the segment in the list
            Segments.Insert(0, s);
        }
        return s;

    }

    public Pieces GetPiece(PieceType pt, int visualIndex)
    {

        Pieces p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject go = null;
            if (pt == PieceType.ramp)
            {
                go = ramps[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblock)
            {
                go = longblocks[visualIndex].gameObject;
            }
            else if (pt == PieceType.jump)
            {
                go = jumps[visualIndex].gameObject;
            }
            else if (pt == PieceType.slide)
            {
                go = slides[visualIndex].gameObject;
            }
            go = Instantiate(go);
            p = go.GetComponent<Pieces>();
            pieces.Add(p);

        }

        return p;
    }
}


