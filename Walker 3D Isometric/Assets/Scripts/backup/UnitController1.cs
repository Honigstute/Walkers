/*
//using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class UnitController : MonoBehaviour
{

    public bool selected;
    public bool inMotion;
    public bool hasMoved;
    GameMaster gm;

    public Transform target;
    private NavMeshPath thePath;
    private float elapsed = 0.0f;

    public NavMeshAgent agent;


    public Vector3 worldPosition;

    public Camera cam;

    private LineRenderer lineRenderer;

    public float movementRange;
    private SphereCollider rangeDetector;


    // Determines from which camera, the positon is converted to ingame coordinates


  
    public ThirdPersonCharacter character;

    private void Start()
    {


        thePath = new NavMeshPath();
        elapsed = 0.0f;

        lineRenderer = GetComponent<LineRenderer>();

        // Checks if Unit is running
        inMotion = false;


        rangeDetector = this.GetComponent<SphereCollider>();
        rangeDetector.enabled = false;

        rangeDetector.radius = movementRange;

        //Accesses the Gamemaster functions
        gm = FindObjectOfType<GameMaster>();


        //we want the animation to rotate the object 
        agent.updateRotation = false;
    }


    private void OnMouseDown()
    {
        
        //If we click on a selected Unit
        if (selected == true)
        {
            
            //Resets the value
            selected = false;
            //Removes the selection
            gm.selectedUnit = null;

            
        } else //if we clicked on a Unit which isn't been selected
        {
            //if there is another Unit in our game selected we have to deselected first
            if (gm.selectedUnit != null) 
            {
                gm.selectedUnit.selected = false;
            }

            selected = true;
            gm.selectedUnit = this;

            
            GetWalkableRange();
        }
    }

    void GetWalkableRange()
    {
        if (hasMoved == true)
        {
            return;
        }

        rangeDetector.enabled = true;

    }


    // Update is called once per frame
    void Update()
    {




        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {

            //Vector3 temp = Input.mousePosition;
            //myMousePos = Camera.main.ScreenToWorldPoint(temp);
            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
            }


            Debug.Log("Hello: " + worldPosition);

            elapsed -= 0.1f;
            NavMesh.CalculatePath(transform.position, worldPosition, NavMesh.AllAreas, thePath);

            
        }
        
        for (int i = 0; i < thePath.corners.Length - 1; i++) {
            
            Debug.DrawLine(thePath.corners[i], thePath.corners[i + 1], Color.red);
            }








        ///////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////// Movement /////////////////////////////////////////////

        //Do we click the left Mousebutton
        if (Input.GetMouseButtonDown(0))
        {
            inMotion = true;
            agent.speed = 6;
            
        } else if(inMotion == false){
           
            //This creates a Ray in our current mouseposition
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            //Stores the information about what we hit
            RaycastHit hit;

            //This shoots out our ray with with out mouse position and stores what we hit, and if we do continues with the if statement
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
                agent.speed = 0;
            } 
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
        }
        
        
    }


}
*/