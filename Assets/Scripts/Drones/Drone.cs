using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{
    /// this is the master script we are using to build all of our drones
    /// all drone functions run off of this one script, different drones will simply have different capabilities defined by this

    // our drone information
    public string droneName; // the name of our drone, ex: Delivery Drone, Construction Drone
    public string droneInventoryString; // what is our drone holding?
    public string currentBehaviourString; // our current behaviour

    // how much can our drone hold?
    public float droneStorage;
    public Dictionary<Building.Itemtypes, float> storedItems = new Dictionary<Building.Itemtypes, float>();

    // the states our drone can be in
    public enum DroneStates
    {
        recharge, delivery, construction, repair, explore, max
    }

    // the list of possible states that our drones can be in
    [SerializeField] List<DroneStates> possibleStates = new List<DroneStates>();
    public DroneStates currentState; // our current drone state
    int currentPossibleState; // our current possible state

    // our position offset for UI reading
    public Vector3 positionOffset;

    // nagivation
    NavMeshAgent navMeshAgent; // our navigation mesh agent

    // UI related
    [SerializeField] GameObject worldSpaceUIParent;
    [SerializeField] Slider progressSlider; // shows the progress of our actions
    float sliderProgressSpeed; // set when we perform an action

    private void Start()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        Invoke("LateStart", 1f);
    }

    void LateStart()
    {
        SetBehaviour();
        // start our quater loop
        StartCoroutine(QuarterLoop());
    }

    // runs four times every second
    IEnumerator QuarterLoop()
    {
        UpdateInventoryInfo();
        yield return new WaitForSecondsRealtime(0.25f);
    }

    // check whether or not we are highlighted by the player's mouse
    private void OnMouseEnter()
    {
        PlayerMouse.instance.highlightedDrone = this;
    }

    private void OnMouseExit()
    {
        PlayerMouse.instance.highlightedDrone = null;
    }

    // set our behaviour based on our current state
    void SetBehaviour()
    {
        // activate the state
        switch(currentState)
        {
            case DroneStates.recharge:
                break;
            case DroneStates.delivery:
                currentBehaviourString = "Delivering";
                StartCoroutine(DeliveryBehaviour());
                break;
            case DroneStates.construction:
                currentBehaviourString = "Constructing";
                StartCoroutine(ConstructionBehaviour());
                    break;
            case DroneStates.repair:
                break;
            case DroneStates.explore:
                currentBehaviourString = "Exploring";
                StartCoroutine(ExploreBehaviour());
                break;
        }
    }

    // move to our next AI state
    void NextState()
    {
        // setting our next possible state
        if (currentPossibleState + 1 < possibleStates.Count)
        {
            currentPossibleState++;
            // then set our current state
            currentState = possibleStates[currentPossibleState];
            return;
        }
        else if (currentPossibleState + 1 >= possibleStates.Count)
        {
            currentPossibleState = 0;
            currentState = possibleStates[0];
            return;
        }
    }

    void UpdateInventoryInfo()
    {
        droneInventoryString = "";
        // add all of our inventory items to it
        foreach (Building.Itemtypes key in storedItems.Keys)
        {
            if (storedItems[key] > 0)
                droneInventoryString += "Holding " + storedItems[key].ToString() + " " + key + "\n";
        }
    }

    // our exploration behaviour
    IEnumerator ExploreBehaviour()
    {
        // find the nearest unscanned tile to us
        float closestDistance = PlanetGenerator.instance.PlanetSize; TileClass closestTile = PlanetGenerator.instance.PlanetTiles[0,0]; 
        foreach (TileClass tile in PlanetGenerator.instance.PlanetTiles)
        {
            if (Vector3.Distance(tile.transform.position, transform.position) < closestDistance && tile.tileScanned == false)
            {
                closestDistance = Vector3.Distance(tile.transform.position, transform.position);
                closestTile = tile;
            }
        }

        float closestPdistance = PlanetGenerator.instance.PlanetSize; TileClass closestPtile = PlanetGenerator.instance.PlanetTiles[0, 0];
        // are there any priority tiles?
        foreach (TileClass tile in PlanetGenerator.instance.PlanetTiles)
        {
            if (Vector3.Distance(tile.transform.position, transform.position) < closestPdistance && tile.tileScanned == false && tile.priorityScan)
            {
                closestPdistance = Vector3.Distance(tile.transform.position, transform.position);
                closestPtile = tile;
                closestTile = closestPtile;
            }
        }

        // move to the tile
        navMeshAgent.SetDestination(new Vector3(closestTile.transform.position.x, transform.position.y, closestTile.transform.position.z));
        // wait until we are at the task location
        yield return new WaitUntil(AtTaskLocation);
        // scan the tile
        // set our UI to active
        worldSpaceUIParent.SetActive(true);
        // begin our progress bar, percent per second, for 5 seconds
        progressSlider.value = 0;
        sliderProgressSpeed = 20;
        StartCoroutine(ProcessSliderVisual());
        // wait for the action to complete
        yield return new WaitUntil(TaskProgressComplete);
        // turn off our UI
        worldSpaceUIParent.SetActive(false);
        // reveal its information
        closestTile.tileScanned = true;
        closestTile.OnScan();
        // move on to the next behaviour
        NextState();
        // start the next state
        SetBehaviour();
        // end the coroutine
        yield break;
    }

    // our delivery behaviour
    IEnumerator DeliveryBehaviour()
    {
        Debug.Log("Delivery check starting...");
        // check our drone request list for open delivery tasks
        DroneRequest request = null;
        for(int i = 0; i < DroneManager.instance.droneRequests.Count; i++)
            if (DroneManager.instance.droneRequests[i].requestType == DroneRequest.RequestTypes.delivery)
            {
                // make sure this isn't assigned to another drone
                if (DroneManager.instance.droneRequests[i].assignedDrone == null)
                {
                    request = DroneManager.instance.droneRequests[i];
                    request.assignedDrone = this;
                }
            }

        // if we don't find anything break out of the coroutine
        if (request == null)
        {
            // choose the next state
            NextState();
            // start the next state
            SetBehaviour();
            yield break;
        }

        // find the requested object in our buildings
        float closestDistance = PlanetGenerator.instance.PlanetSize; Building closestBuilding = null; 
        foreach (Building building in BuildingManager.instance.buildings)
        {
            // if this building has the item we need and it is not an input only item on that building then we can take it
            if (building.storedItems.ContainsKey(request.requestedItem) && !building.inputOnlyItems.Contains(request.requestedItem) && building.storedItems[request.requestedItem] > request.requestedAmount)
            {
                // if this is closest...
                if (Vector3.Distance(transform.position, building.transform.position) < closestDistance)
                {
                    closestDistance = Vector3.Distance(transform.position, building.transform.position);
                    closestBuilding = building;
                }
            }
        }

        // go to our closest building to retrieve the item
        navMeshAgent.SetDestination(new Vector3(closestBuilding.transform.position.x, transform.position.y, closestBuilding.transform.position.z));

        // wait until we are at the location to pickup our items
        yield return new WaitUntil(AtTaskLocation);
        
        // when we are at the location pickup the items
        if (closestBuilding.storedItems.ContainsKey(request.requestedItem) && closestBuilding.storedItems[request.requestedItem] >= request.requestedAmount)
        {
            // then transfer the item to us
            closestBuilding.storedItems[request.requestedItem] -= request.requestedAmount;
            if (!storedItems.ContainsKey(request.requestedItem))
                storedItems.Add(request.requestedItem, request.requestedAmount);
            else
            {
                storedItems[request.requestedItem] += request.requestedAmount;
            }
            // items picked up
            Debug.Log("items picked up");

        }
        else // if in the process of getting to the building we don't have the items anymore, we have to find a new task
        {
            // move to the next behaviour
            NextState();
            // start the next state
            SetBehaviour();
            yield break;
        }

        // if we are holding all of the items we need, go to our destination to deliver them
        // are we going to a building or to a tile?
        if (request.receivingBuilding != null)
            navMeshAgent.SetDestination(new Vector3(request.receivingBuilding.transform.position.x, transform.position.y, request.receivingBuilding.transform.position.z));

        /* // we are not doing tile deliveries anymore!
        else if (request.receivingTileClass != null)
            navMeshAgent.SetDestination(new Vector3(request.receivingTileClass.transform.position.x, transform.position.y, request.receivingTileClass.transform.position.z));*/

        // then wait for the drone to get to the location
        yield return new WaitUntil(AtTaskLocation);

        // then deliver the items
        if (request.receivingBuilding != null)
        {
            // debug
            Debug.Log("items delivered");

            // add to the building
            if (request.receivingBuilding.storedItems.ContainsKey(request.requestedItem))       
            {
                // give the items
                request.receivingBuilding.storedItems[request.requestedItem] += request.requestedAmount;
                storedItems[request.requestedItem] -= request.requestedAmount;
            } else if (!request.receivingBuilding.storedItems.ContainsKey(request.requestedItem))
            {
                // add the key
                request.receivingBuilding.storedItems.Add(request.requestedItem, storedItems[request.requestedItem]);

                // remove from ourselves
                storedItems[request.requestedItem] -= request.requestedAmount;
            }
            // we have successfully completed the task of delivery
            request.CompleteRequest();
        }

        // advance our state
        NextState();
        // set our next state
        SetBehaviour();
        yield return null;  
    }

    // our construction behaviour
    IEnumerator ConstructionBehaviour()
    {
        // find a construction request
        // check our drone request list for open construction tasks
        DroneRequest request = null;
        for (int i = 0; i < DroneManager.instance.droneRequests.Count; i++)
            if (DroneManager.instance.droneRequests[i].requestType == DroneRequest.RequestTypes.construction)
            {
                // is the request ready for construction
                // make sure this isn't assigned to another drone
                if (DroneManager.instance.droneRequests[i].assignedDrone == null && DroneManager.instance.droneRequests[i].receivingBuilding.readyForConstructionLabor)
                {
                    request = DroneManager.instance.droneRequests[i];
                    request.assignedDrone = this;
                }
            }

        // if there are no construction requests available
        if (request == null)
        {
            NextState();
            SetBehaviour();
            yield break;
        }

        // go to our construction site
        navMeshAgent.SetDestination(new Vector3(request.receivingBuilding.transform.position.x, transform.position.y, request.receivingBuilding.transform.position.z));

        // wait until we get there
        yield return new WaitUntil(AtTaskLocation);

        // set our UI to active
        worldSpaceUIParent.SetActive(true);
        // perform our construction labor
        progressSlider.value = 0;
        sliderProgressSpeed = 100/request.constructionSite.constructionLabor; // get the amount of seconds in %
        StartCoroutine(ProcessSliderVisual());
        // wait until the task is complete
        yield return new WaitUntil(TaskProgressComplete);
        // complete the construction
        request.constructionSite.constructionLabor = 0;
        // set our UI to active
        worldSpaceUIParent.SetActive(false);
        // once the task is complete, go to the next task
        NextState();
        SetBehaviour();
    }

    // reports on the progress the drone has made on its task
    bool TaskProgressComplete()
    {
        return progressSlider.value == progressSlider.maxValue;
    }

    // returns whether this drone has arrived at its task location
    bool AtTaskLocation()
    {
        // only check on the x and z axes
        return Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z)) < 0.1f;
    }

    // process our slider visual
    IEnumerator ProcessSliderVisual()
    {
        if (progressSlider.value != progressSlider.maxValue)
        {
            // make the bar face the camera
            progressSlider.transform.LookAtCamera();
            // update the value
            progressSlider.value += Time.deltaTime * sliderProgressSpeed;
            // start again, run ever second
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(ProcessSliderVisual());
        }
        yield break;
    }
}
