using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    // setup our instance
    public static PlayerMouse instance;
    private void Awake() => instance = this;
    /// this script manages our player's mouse, the current tile we are hovering, and the current tile we have selected
    [SerializeField] GameObject mouseSelector; // the selector that shows which tile we are hovering over
    [SerializeField] Vector3 targetPosition; // the position we lerp to
    [SerializeField] float lerpSpeed; // how fast we lerp to our target positions
    public TileClass highlightedTile; // which tile have we highlighted?

    private void Start()
    {
        // set the tile to the first tile in the set so that we don't get errors on startup
        highlightedTile = GrabTileInfo(1, 1);
    }

    // use this to grab info from a tile
    TileClass GrabTileInfo(int x, int y)
    {
        // get the tile
        return PlanetGenerator.instance.PlanetTiles[x, y];
    }

    private void FixedUpdate()
    {
        // update our target positions
        targetPosition = highlightedTile.transform.position;
        // lerp to our target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        ProcessClicks();
    }

    void ProcessClicks()
    {
        // process left clicks
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
        }
    }

    // what happens when we click?
    void OnLeftClick()
    {
        // if we click and have a highlighted tile
        if (highlightedTile)
        {
            CreateTileInfoPopup(highlightedTile);
        }
    }

    // create a tile info popup for the player to read
    [SerializeField] GameObject tileInfoPopupPrefab; // the prefab we are using to build the popups
    void CreateTileInfoPopup(TileClass tile)
    {
        // clear our UI elements
        PlayerUIManager.instance.ClearDynamicUI();

        // instantiate the prefab at the selector's point
        TileInfoPopup tip = Instantiate(tileInfoPopupPrefab, transform.position, Quaternion.identity, tile.transform).GetComponent<TileInfoPopup>();
        // add this new UI element to the active dynamic UI elements on the manager
        PlayerUIManager.instance.ActiveDynamicUIElements.Add(tip.gameObject);
        // send the tile info
        tip.selectedTile = tile;
    }

}
