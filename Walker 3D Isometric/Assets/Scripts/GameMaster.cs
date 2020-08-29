using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    public UnitController selectedUnit;

    public int playerTurn = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EndTurn();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    /////////////////////////// End of Turn ///////////////////////////////////////
    
    void EndTurn()
    {
        if (playerTurn == 1)
        {
            playerTurn = 2;
        } else if (playerTurn == 2)
        {
            playerTurn = 1;
        }

        if (selectedUnit != null)
        {
            selectedUnit.selected = false;
            selectedUnit = null;
            
        }

        foreach (UnitController unitController in FindObjectsOfType<UnitController>())
        {
            unitController.transform.Find("Sphere").gameObject.SetActive(false); //RED SELECTION SPHERE
            unitController.actionPoints = unitController.actionPointsMaximum;
        }
    }
}
