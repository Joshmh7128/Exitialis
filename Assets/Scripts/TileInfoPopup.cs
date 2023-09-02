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

    // start runs when the object first exists in the world
    private void Start()
    {
        // look at the camera
        canvasParent.LookAtCamera();
        // at the very start of us, get the information of the selected tile
        GetTileInfo();
        // update this tile's information panel every second
        StartCoroutine(UpdateTileInfo());
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
