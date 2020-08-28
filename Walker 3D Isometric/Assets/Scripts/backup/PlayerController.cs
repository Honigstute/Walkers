
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{
    // Determines from which camera, the positon is converted to ingame coordinates
    public Camera cam;


    // Refrence to the agent
    public NavMeshAgent agent;

    // 
    public ThirdPersonCharacter character;

    private void Start()
    {
        //we want the animation to rotate the object 
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Do we click the left Mousebutton
        if(Input.GetMouseButtonDown(0))
        {
            //This creates a Ray in our current mouseposition
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            //Stores the information about what we hit
            RaycastHit hit;

            //This shoots out our ray with with out mouse position and stores what we hit, and if we do continues with the if statement
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

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
