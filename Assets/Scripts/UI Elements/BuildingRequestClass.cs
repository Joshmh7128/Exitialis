using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRequestClass : MonoBehaviour
{
    public TileClass parentClass;
    public GameObject buildingPrefab; // the prefab we want to build
    // for our scan request dynamic UI element
    public UnityEngine.UI.Toggle toggle;

    bool isMouseOver; // is the mouse over?

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void Update()
    {
        // check if we are clicked
        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Test");
            toggle.isOn = !toggle.isOn;
        }
    }

    // for the onchange event of our toggle
    public void SetBuildingRequest()
    {
        parentClass.RequestBuilding(buildingPrefab);
    }
}
