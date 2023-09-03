using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoPopup : MonoBehaviour
{
    /// script grabs and populates an info popup panel when a tile is selected
    public TileClass selectedTile; // the tile we are currently reading
    [SerializeField] Transform canvasParent; // our canvas parent
    [SerializeField] Text tileNameDisplay, tileFlavorDisplay; // displays the tile's name
    [SerializeField] GameObject scanInfoRequestPrefab; // our request to spawn on an object if it has not been scanned
    [SerializeField] GameObject buildingRequestPrefab; // the button we can push to request buildings
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

    // while the mouse is over, set the highlighted tile to our selected tile so that we can't lose focus easily
    private void OnMouseEnter()
    {
        PlayerMouse.instance.highlightedTile = selectedTile;
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
        
        // if this is a scanned tile then setup a building request
        if (selectedTile.tileScanned)
        {
            BuildingRequestClass building = Instantiate(buildingRequestPrefab, requestGroup).GetComponent<BuildingRequestClass>();
            building.parentClass = selectedTile;
            building.toggle.isOn = selectedTile.buildingRequested;
            // for the sake of this, we can only build the test building
            building.buildingPrefab = BuildingManager.instance.buildingPrefabs[(int)BuildingManager.BuildingTypes.test];
        }
    }

    // get our tile info
    void GetTileInfo()
    {
        // has this tile been scanned by a drone?
        if (selectedTile.tileScanned)
        {
            // remove the scan priority request
            foreach (Transform t in requestGroup)
            {
                if (t.gameObject.GetComponent<ScanRequestClass>())
                    Destroy(t.gameObject);
            }

            // set our display name
            tileNameDisplay.text = selectedTile.tileName;
            // set our info
            tileFlavorDisplay.text = selectedTile.tileFlavorText;
        }
        else
        {
            // set our display name
            tileNameDisplay.text = "Unscanned";
            tileFlavorDisplay.text = "";
        }
    }

    // update every second
    IEnumerator UpdateTileInfo()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        GetTileInfo();
        StartCoroutine(UpdateTileInfo());
    }

}
