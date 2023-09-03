using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles

    [Header("Tile Information")]
    public string tileName, tileFlavorText; // our tile's name, ex: Grass Tile, Desert Tile
    public bool tileScanned, priorityScan, hasBuilding, buildingRequested; // has this tile been scanned? does it want a priority scan? does this tile have a building?
    [SerializeField] MeshRenderer tileRenderer;

    // items we can have delivered for building construction
    public Dictionary<Building.Itemtypes, float> storedItems = new Dictionary<Building.Itemtypes, float>();

    private void Start()
    {
        CheckScan();
    }

    private void OnMouseEnter()
    {
        // set this to our active tile
        PlayerMouse.instance.highlightedTile = this;
    }

    // the scan check we perform at the start of play
    void CheckScan()
    {
        // if we are not scanned set our material to dark
        if (!tileScanned)
            tileRenderer.material.color = Color.white;
    }

    // anything that happens on scan
    public void OnScan()
    {
        // when we become scanned set our material to white
        tileRenderer.material.color = Color.white;
    }

    // put in a building request
    public void RequestBuilding(GameObject buildingPrefab)
    {
        if (buildingRequested) return; // don't continue if we already have requested a building on this tile
        // we have requested a building
        buildingRequested = true;
        // get our building information
        Building building = buildingPrefab.GetComponent<Building>();
        // create a new building request
        DroneRequest droneRequest = new DroneRequest();
        // build the request
        droneRequest.constructableBuilding = buildingPrefab;
        droneRequest.receivingTileClass = this;
        droneRequest.requestType = DroneRequest.RequestTypes.construction;
        // add our building requirements
        foreach (Building.Itemtypes item in building.itemRequiredConstructionTypes)
            droneRequest.constructionRequirements.Add(item, building.itemRequiredConstructionCounts[building.itemRequiredConstructionTypes.IndexOf(item)]);
        // add this request to the drone manager
        DroneManager.instance.droneRequests.Add(droneRequest);
        droneRequest.CreateDeliveriesForConstruction();
    }

    // begin construction
    public void ConstructBuilding(GameObject buildingPrefab)
    {

    }
}
