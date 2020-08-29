using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHover : MonoBehaviour
{

    private void OnMouseEnter()
    {
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void OnMouseExit()
    {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

}
