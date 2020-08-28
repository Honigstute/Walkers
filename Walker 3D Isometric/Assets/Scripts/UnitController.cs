
//using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class UnitController : MonoBehaviour
{
    //Timers
    public float selectionTimer;

    public bool activateMovement;
    public bool initiateMovement;
    public bool selected;
    public bool hasMoved;
    public int playerNumber;
    public NavMeshAgent agent;
    GameMaster gm;
    private NavMeshPath thePath;
    private float elapsed = 0.0f;
    public Vector3 worldPosition;
    public Camera cam;
    public ThirdPersonCharacter character;

    public float actionPoints;
    public Text textActionPoints;
    
    public float moveRange;
    public float calMoveRange;
    public Text textMoveRange;

    private LineRenderer lineRendererPath;


    private void Start()
    {
        activateMovement = false;
        initiateMovement = false;

        //HUD
        actionPoints = 23;
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



    private void OnMouseDown()
    {
        
        if (selected == true) //If we click on a selected Unit
        {
            Debug.Log("fuck");
            selected = false; //Resets the value
            gm.selectedUnit = null; //Removes the selection
            transform.Find("Sphere").gameObject.SetActive(false);

        } else //if we clicked on a Unit which isn't been selected
        {
            
            //if there is another Unit in our game selected we have to deselected first
            if (gm.selectedUnit != null) 
            {
                
                gm.selectedUnit.selected = false;
                gm.selectedUnit.transform.Find("Sphere").gameObject.SetActive(false);

            }
            
            selected = true;
            gm.selectedUnit = this;

            transform.Find("Sphere").gameObject.SetActive(true);

            GetWalkableRange();
        }
    }

    void GetWalkableRange()
    {
        if (hasMoved == true)
        {
            return;
        }
    }


    void Update()
    {

        moveRange = agent.GetPathRemainingDistance();
        calMoveRange = thePath.GetPathRemainingDistance2();
        // Debug.Log("Hello: " + calMoveRange);
        // Debug.Log(agent.GetPathRemainingDistance());

        if (gm.selectedUnit == this)
        {

            ///////////////////////////////////////////////////////////////////////////////////////////
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

            ///////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// SetPath //////////////////////////////////////////////

            textActionPoints.text = "Action Points: " + Mathf.Round(actionPoints * 10) / 10;

            Debug.Log(actionPoints);
            if (agent.isStopped == true && calMoveRange <= actionPoints) // Iam a running then dont execute
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
            lineRendererPath.enabled = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////// Animation ////////////////////////////////////////////

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
}



