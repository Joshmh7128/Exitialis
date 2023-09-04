using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneInfoPopup : MonoBehaviour
{
    /// script grabs and populates an info popup panel when a tile is selected
    public Drone selectedDrone; // the tile we are currently reading
    [SerializeField] Transform canvasParent; // our canvas parent
    [SerializeField] Text droneNameDisplay, droneStateDisplay, droneInventoryDisplay; // displays the drone information

    // start runs when the object first exists in the world
    private void Start()
    {
        // look at the camera
        canvasParent.LookAtCamera();
        // at the very start of us, get the information of the selected tile
        GetDroneInfo();
        // start our update
        StartCoroutine(UpdateDroneInfo());
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
        droneNameDisplay.text = selectedDrone.droneName;
        // set our current ai state
        droneStateDisplay.text = selectedDrone.currentBehaviourString;
        // set our inventory
        droneInventoryDisplay.text = selectedDrone.droneInventoryString;
    }

    // update every second
    IEnumerator UpdateDroneInfo()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetDroneInfo();
        StartCoroutine(UpdateDroneInfo());
    }
}
