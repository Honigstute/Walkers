
//using System.Diagnostics;
//using Boo.Lang;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class UnitController : MonoBehaviour
{

    public List<UnitController> enemiesInRange = new List<UnitController>();


    public float actionPoints;
    public float actionPointsMaximum; 
    public Text textActionPoints;

    public float attackRange;
    [SerializeField]
    Transform destination;

    public float enemyRange;
    public float moveRange;
    public float calMoveRange;
    public Text textMoveRange;

    //Timers
    public float selectionTimer; //Seperates the first selection click from the moving to position

    public bool activateMovement; //Unlocks movement
    public bool initiateMovement; //Prevents the unit from moving by trying to selecting it
    public bool selected; //Marks a Unit as selected
    public bool hasMoved; //Not in use
    public int playerNumber; //Changes Team

    public NavMeshAgent agent;
    GameMaster gm;
    private NavMeshPath thePath;
    private float elapsed = 0.0f;
    public Vector3 worldPosition;
    public Camera cam;
    public ThirdPersonCharacter character;
    private LineRenderer lineRendererPath;

    private bool something = true;

    private void Start()
    {
        activateMovement = false;
        initiateMovement = false;

        //HUD
        actionPoints = actionPointsMaximum;
        textActionPoints.text = "Action Points " + actionPoints.ToString();

        //Timers
        selectionTimer = 0.2f;

        thePath = new NavMeshPath();
        elapsed = 0.0f;

        lineRendererPath = GetComponent<LineRenderer>(); // The Path

        //Accesses the Gamemaster functions
        gm = FindObjectOfType<GameMaster>();

        //The Animation takes rotation into account
        agent.updateRotation = false;

        //Displays the red selection ball
        transform.Find("Sphere").gameObject.SetActive(false);
    }



    ///////////////////////////////////////////////////////////////////////////////
    /////////////////////////// Selection Section /////////////////////////////////
    private void OnMouseDown()
    {
        
        if (selected == true) //We click on a selected Unit
        {
            //Resetting Values
            selected = false;
            gm.selectedUnit = null; 
            transform.Find("Sphere").gameObject.SetActive(false); //RED SELECTION SPHERE (would be removed)

        } else //We clicked on a unselected Unit 
        {

            /////////////////////////// Switches between Player round and Enemy round ///////////////////////////////////////
            
            if (playerNumber == gm.playerTurn)
            {
                if (gm.selectedUnit != null) //Deselect any other Unit if I choose new Unit
                {
                    gm.selectedUnit.selected = false;
                    gm.selectedUnit.transform.Find("Sphere").gameObject.SetActive(false); //RED SELECTION SPHERE (would be removed)
                }

                selected = true;
                gm.selectedUnit = this;
                transform.Find("Sphere").gameObject.SetActive(true); //RED SELECTION SPHERE (would be removed)

                GetEnemies();

                GetWalkableRange();
            }
        }
    }

    void GetWalkableRange()
    {
        if (hasMoved == true)
        {
            return;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    /////////////////////////// Selection Section End /////////////////////////////


    ///////////////////////////////////////////////////////////////////////////////
    /////////////////////////// Attack Section ////////////////////////////////////
    
    void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach (UnitController unitController in FindObjectsOfType<UnitController>())
        {
            /////////////////////////// Get the enemys which are in range ///////////////////////////////transform.localScale = Vector3.zero;
            
            if (something == true) //Enter the attackrange here
            {

                Debug.Log("leckerlecker");
                if (unitController.playerNumber != gm.playerTurn) //Avoid Friendly Fire and add if we have sufficient Ap points to attack
                {
                   
                    enemiesInRange.Add(unitController);
                }
            }
        }
    }







    void Update()
    {
        enemyRange = thePath.GetPathRemainingDistancetoEnemies();
        moveRange = agent.GetPathRemainingDistance();
        calMoveRange = thePath.GetPathRemainingDistance2();

        if (gm.selectedUnit == this)
        {

            //////////////////////////////////// Calculated Path //////////////////////////////////////
            
            if (agent.isStopped == true) { 

                RaycastHit distance;
                Ray ray2 = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray2, out distance))
                {
                    worldPosition = distance.point;
                }

                NavMesh.CalculatePath(transform.position, worldPosition, NavMesh.AllAreas, thePath);
          
                for (int i = 0; i < thePath.corners.Length - 1; i++) { Debug.DrawLine(thePath.corners[i], thePath.corners[i + 1], Color.red); } // Draws the calculate Path
            }

            //////////////////////////////////// SetPath //////////////////////////////////////////////

            textActionPoints.text = "Action Points: " + Mathf.Round(actionPoints * 10) / 10;

            Debug.Log(actionPoints);
            if (agent.isStopped == true && calMoveRange <= actionPoints) //Pathline dosen't redraw until Unit arrives destination 
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit)) 
                {
                    agent.SetDestination(hit.point);
                }
                // Displaying the Path
                lineRendererPath.positionCount = agent.path.corners.Length;
                lineRendererPath.SetPositions(agent.path.corners);
                lineRendererPath.enabled = true;
            } else if(agent.isStopped == true && calMoveRange >= actionPoints)
            {
                agent.ResetPath(); // Fix for too much Movement range
                lineRendererPath.enabled = false; 
            }

            if (actionPoints <= 1) // switches of the line renderer if dont habe AP anymore
            {
                lineRendererPath.enabled = false;
            }

                // If this timer goes up else happens
            if (selectionTimer >= 0) {
                agent.isStopped = true;
                selectionTimer -= Time.deltaTime;} 
            else {
                if (Input.GetMouseButtonDown(0) && agent.isStopped == true && lineRendererPath.enabled == true)
                {
                    actionPoints -= moveRange;
                    initiateMovement = true;
                    agent.isStopped = false;
                }
                textMoveRange.text = "Move Range: " + Mathf.Round(moveRange * 10) / 10;
            }

        } else
        {

            ///////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////// Resetting Area after deselection ////////////////////////////
            
            initiateMovement = false;
            lineRendererPath.enabled = false;
            selectionTimer = 0.2f; 
        }

        //statement checks if we reached our destination 
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);

        } else
        {   //Stops player
            
            character.Move(Vector3.zero, false, false);
            agent.isStopped = true;
        }  
    }

}


///////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////// RangeChecker /////////////////////////////////////////

public static class ExtensionMethods
{
    public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || navMeshAgent.path.corners.Length == 0)
        {
            return -1f;
        }


        float distance = 0.0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }
        //Debug.Log(distance);
        return distance;
        
    }

    public static float GetPathRemainingDistance2(this NavMeshPath navMeshPath)
    {
        if (navMeshPath.status != NavMeshPathStatus.PathComplete || navMeshPath.corners.Length == 0)
        {
            return 100f;
        }

        float distance = 0.0f;
        for (int i = 0; i < navMeshPath.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
        }
        
        //Debug.Log(distance);
        return distance;
    }

    public static float GetPathRemainingDistancetoEnemies(this NavMeshPath navMeshPath)
    {
        if (navMeshPath.status != NavMeshPathStatus.PathComplete || navMeshPath.corners.Length == 0)
        {
            return 100f;
        }

        float distance = 0.0f;
        for (int i = 0; i < navMeshPath.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
        }

        //Debug.Log(distance);
        return distance;
    }
}



