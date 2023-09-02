using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoPopup : MonoBehaviour
{
    /// script grabs and populates an info popup panel when a tile is selected
    public TileClass selectedTile; // the tile we are currently reading
    [SerializeField] Transform canvasParent; // our canvas parent
    [SerializeField] Text tileNameDisplay; // displays the tile's name
    [SerializeField] GameObject scanInfoRequestPrefab; // our request to spawn on an object if it has not been scanned
    [SerializeField] Transform requestGroup; // the group that handles all of our requests

    // start runs when the object first exists in the world
    private void Start()
    {
        // look at the camera
        canvasParent.LookAtCamera();
        // at the very start of us, get the information of the selected tile
        GetTileInfo();
        // setup our requests
        SetupRequests();
        // update this tile's information panel every second
        StartCoroutine(UpdateTileInfo());
    }

    // setup our requests
    void SetupRequests()
    {
        // add in a scan request
        if (!selectedTile.tileScanned)
        {
            ScanRequestClass scan = Instantiate(scanInfoRequestPrefab, requestGroup).GetComponent<ScanRequestClass>();
            scan.parentClass = selectedTile;
            scan.toggle.isOn = selectedTile.priorityScan;
        }
    }

    // get our tile info
    void GetTileInfo()
    {
        // has this tile been scanned by a drone?
        if (selectedTile.tileScanned)
        {
            // set our display name
            tileNameDisplay.text = selectedTile.tileName;
        }
        else
        {
            // set our display name
            tileNameDisplay.text = "Unscanned";
        }
    }

    // update every second
    IEnumerator UpdateTileInfo()
    {
        yield return new WaitForSecondsRealtime(1f);
        GetTileInfo();
        StartCoroutine(UpdateTileInfo());
    }

}
