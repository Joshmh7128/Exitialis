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
    [SerializeField] Vector3 targetPosition, targetScale; // the position we lerp to
    [SerializeField] float lerpSpeed; // how fast we lerp to our target positions
    public TileClass highlightedTile; // which tile have we highlighted?
    public Drone highlightedDrone; // is a drone highlighted?
    public Building highlightedBuilding; // is a building highlighted?

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
        if (highlightedTile && !highlightedDrone)
        {
            targetPosition = highlightedTile.transform.position;
            targetScale = Vector3.one;
        }

        if (highlightedBuilding)
        {
            targetPosition = highlightedBuilding.transform.position;
            targetScale = new Vector3(0.5f, 1, 0.5f);
        }

        if (highlightedDrone)
        {
            targetPosition = highlightedDrone.transform.position + highlightedDrone.positionOffset;
            targetScale = new Vector3(0.5f, 1, 0.5f);
        }
        // lerp to our target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.fixedDeltaTime);
        // lerp to our target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.fixedDeltaTime);
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
        if (highlightedTile && !highlightedDrone)
        {
            CreateTileInfoPopup(highlightedTile);
        }

        // if we have clicked on a highlighted drone
        if (highlightedDrone)
        {
            CreateDroneInfoPopup(highlightedDrone);
        }

        // if we have clicked on a highlighted building
        if (highlightedBuilding)
        {
            CreateBuildingInfoPopup(highlightedBuilding);
        }
    }

    // create a tile info popup for the player to read
    [SerializeField] GameObject tileInfoPopupPrefab; // the prefab we are using to build the popups
    void CreateTileInfoPopup(TileClass tile)
    {
        // clear our UI elements
        PlayerUIManager.instance.ClearDynamicUI();

        // instantiate the prefab at the selector's point
        TileInfoPopup tip = Instantiate(tileInfoPopupPrefab, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<TileInfoPopup>();
        // add this new UI element to the active dynamic UI elements on the manager
        PlayerUIManager.instance.ActiveDynamicUIElements.Add(tip.gameObject);
        // send the tile info
        tip.selectedTile = tile;
    }

    // create a drone info popup for the player to read
    [SerializeField] GameObject droneInfoPopupPrefab; // the prefab we are using to build the popups
    void CreateDroneInfoPopup(Drone drone)
    {
        // clear our UI elements
        PlayerUIManager.instance.ClearDynamicUI();

        // instantiate the prefab at the selector's point
        DroneInfoPopup dip = Instantiate(droneInfoPopupPrefab, drone.transform.position + drone.positionOffset, Quaternion.identity, drone.transform).GetComponent<DroneInfoPopup>();
        // add this new UI element to the active dynamic UI elements on the manager
        PlayerUIManager.instance.ActiveDynamicUIElements.Add(dip.gameObject);
        // send the tile info
        dip.selectedDrone = drone;
    }

    // create a building info popup for the player to read
    [SerializeField] GameObject buildingInfoPopupPrefab; // the prefab we will use to build our building info popup
    void CreateBuildingInfoPopup(Building building)
    {
        // clear our UI
        PlayerUIManager.instance.ClearDynamicUI();

        // instantiate the UI prefab
        BuildingInfoPopup bip = Instantiate(buildingInfoPopupPrefab, building.transform.position, Quaternion.identity, building.transform).GetComponent<BuildingInfoPopup>();
        // add this to the active dynamic UIelements
        PlayerUIManager.instance.ActiveDynamicUIElements.Add(bip.gameObject);
        // send the information to the info popup
        bip.selectedBuilding = building;
    }

}
