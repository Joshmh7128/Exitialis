using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneInfoPopup : MonoBehaviour
{
    /// script grabs and populates an info popup panel when a tile is selected
    public Drone selectedDrone; // the tile we are currently reading
    [SerializeField] Transform canvasParent; // our canvas parent
    [SerializeField] Text tileNameDisplay; // displays the tile's name

    // start runs when the object first exists in the world
    private void Start()
    {
        // look at the camera
        canvasParent.LookAtCamera();
        // at the very start of us, get the information of the selected tile
        GetTileInfo();
    }

    // get our tile info
    void GetTileInfo()
    {
        // set our display name
        tileNameDisplay.text = selectedDrone.droneName;
    }
}
