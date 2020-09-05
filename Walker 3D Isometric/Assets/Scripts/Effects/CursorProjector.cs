using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorProjector : MonoBehaviour
{

    public Camera cam;
    public Vector3 altredPos;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        altredPos = transform.position;
        altredPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 4f);

        Ray rayForProjector = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitForProjector;

        if (Physics.Raycast(rayForProjector, out hitForProjector))
        {
            altredPos = hitForProjector.point;
        }

    }
}
