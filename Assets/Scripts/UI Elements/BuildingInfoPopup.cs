using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPopup : MonoBehaviour
{
    /// script grabs and populates an info popup panel when a tile is selected
    public Building selectedBuilding; // the tile we are currently reading
    [SerializeField] Transform canvasParent; // our canvas parent
    [SerializeField] Text buildingNameDisplay, buildingInfoDisplay; // displays the tile's name

    // start runs when the object first exists in the world
    private void Start()
    {
        // look at the camera
        canvasParent.LookAtCamera();
        // at the very start of us, get the information of the selected tile
        GetDroneInfo();
        // setup the building request buttons we can build here
    }

    // make sure we update our view every frame so that we can see the drone's status
    private void FixedUpdate()
    {
        canvasParent.LookAtCamera();
    }

    // get our tile info
    void GetDroneInfo()
    {
        // set our display name
        buildingNameDisplay.text = selectedBuilding.buildingName;
        // set our current ai state
        buildingInfoDisplay.text = selectedBuilding.buildingInfo;
    }


    // update every second
    IEnumerator UpdateDroneInfo()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetDroneInfo();
        StartCoroutine(UpdateDroneInfo());
    }
}
