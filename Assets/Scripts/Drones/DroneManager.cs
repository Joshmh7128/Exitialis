using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // instance
    public static DroneManager instance;
    private void Awake() => instance = this;
    /// this exists to manage all of our drones and handle drone requests
    public List<DroneRequest> droneRequests = new List<DroneRequest>(); // all of the requests

}

public class DroneRequest
{
    public Drone assignedDrone; // this is null until a drone picks up the task

    public enum RequestTypes
    {
        explore, delivery, construction, repair
    }

    public RequestTypes requestType;

    // for our deliveries
    public Building.Itemtypes requestedItem; // the item the building wants
    public float requestedAmount;
    public Building receivingBuilding = null; // the building asking for the item

    // for our construction
    public GameObject constructableBuilding; // the building prefab we want to build
    public Dictionary<Building.Itemtypes, float> constructionRequirements = new Dictionary<Building.Itemtypes, float>(); // what we need in order to build this building
    public TileClass receivingTileClass = null;

    public bool CheckConstructionViability()
    {
        bool canBuild = true; // can we build it?
        // does this tile have all the resources we need to build the building?
        foreach(Building.Itemtypes item in constructionRequirements.Keys)
        {
            if (receivingTileClass.storedItems.ContainsKey(item) && receivingTileClass.storedItems[item] < constructionRequirements[item])
                canBuild = false;
        }

        return canBuild;
    }

    // for construction, add deliveries for the items we need
    public void CreateDeliveriesForConstruction()
    {
        foreach(var item in constructionRequirements)
        {
            DroneRequest droneRequest = new DroneRequest();
            droneRequest.requestType = RequestTypes.delivery;
            droneRequest.requestedItem = item.Key;
            droneRequest.requestedAmount = item.Value;
            droneRequest.receivingTileClass = receivingTileClass;
            // add this request to the list
            DroneManager.instance.droneRequests.Add(droneRequest);
        }
    }

    // complete this request
    public void CompleteRequest()
    {
        DroneManager.instance.droneRequests.Remove(this);
        Debug.Log(DroneManager.instance.droneRequests);
    }
}