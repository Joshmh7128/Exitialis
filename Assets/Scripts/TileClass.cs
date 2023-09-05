using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles

    [Header("Tile Information")]
    public string tileName, tileFlavorText; // our tile's name, ex: Grass Tile, Desert Tile
    public bool tileScanned, priorityScan, hasBuilding, buildingRequested; // has this tile been scanned? does it want a priority scan? does this tile have a building?
    [SerializeField] MeshRenderer tileRenderer;
    public NavMeshSurface navMeshSurface; // our nav mesh surface

    // items we can have delivered for building construction
    public Dictionary<Building.Itemtypes, float> storedItems = new Dictionary<Building.Itemtypes, float>();

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
        buildingRequested = true;
        if (hasBuilding) return;
        hasBuilding = true;
        // place our building as a construction site on the tile, the building will create a construction request for the items it needs
        Instantiate(buildingPrefab, transform.position, Quaternion.identity, transform);
    }

    // begin construction
    public void ConstructBuilding(GameObject buildingPrefab)
    {

    }
}
