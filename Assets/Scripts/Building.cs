using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    /// this script is for all buildings in the game

    // our building information
    public string buildingName;
    public string buildingFlavor;
    public string buildingInfo; // holds things like storage amounts, power consumption, etc.

    // storage
    public float storageMaximum; // how many items this building can hold in total

    // all of the possible things that can be stored in buildings, tracked
    public Dictionary<Itemtypes, float> storedItems = new Dictionary<Itemtypes, float>();

    // our building requirements in two lists to make them easier to access in the editor
    public List<Itemtypes> itemRequiredConstructionTypes = new List<Itemtypes>();
    public List<float> itemRequiredConstructionCounts = new List<float>();
    public List<Itemtypes> inputOnlyItems = new List<Itemtypes>(); // items that can ONLY be input to this building

    // has this building been constructed? this building will be a "construction site" until it receives all of the resources it needs
    public bool buildingConstructed;
    public bool constructionRequested, deliveriesRequested; // have we requested our construction?

    // for our starting building items
    public List<Itemtypes> startingItems = new List<Itemtypes>();
    public List<float> startingItemCounts = new List<float>();

    // what can this building output?
    public enum Itemtypes
    {
        none, coal, ore, metal
    }

    // what does this building output?
    public Itemtypes outputType;

    // what does this building input?
    public Itemtypes inputType;

    // does this building require power?
    public float powerRequirement, powerInput, powerConsumption;

    // our production requirements
    public float inputRequirement, outputAmount, productionTime;

    // check whether or not we are highlighted by the player's mouse
    private void OnMouseEnter()
    {
        PlayerMouse.instance.highlightedBuilding = this;
    }

    private void OnMouseExit()
    {
        PlayerMouse.instance.highlightedBuilding = null;
    }

    private void Start()
    {
        // setup our building
        SetupBuilding();
        // begin our quarter update
        StartCoroutine(QuarterUpdate());
    }

    void SetupBuilding()
    {
        // add ourselves to the building manager
        BuildingManager.instance.buildings.Add(this);

        // setup our starting items
        foreach (var item in startingItems)
        {
            storedItems[item] = startingItemCounts[startingItems.IndexOf(item)];
        }

        // request our construction if we have not been built
        if (!buildingConstructed)
            RequestConstruction();
    }

    // we need to be built. this function is run when a construction site has been placed
    void RequestConstruction()
    {
        // only run if we need to build
        if (constructionRequested) return;
        // we're here now
        constructionRequested = true;
        // create a new delivery asking for that item at its count
        DroneRequest request = new DroneRequest(); // create a new request
        // copy our construction requirements
        foreach (var requirement in itemRequiredConstructionTypes)
            request.constructionRequirements.Add(requirement, itemRequiredConstructionCounts[itemRequiredConstructionTypes.IndexOf(requirement)]);
        request.requestType = DroneRequest.RequestTypes.construction; // set it to be a construction task so that the delivery tasks can be requested
        request.receivingBuilding = this; // ask for the stuff we need
        DroneManager.instance.droneRequests.Add(request);
        // then ask the task to create the delivery requests
        request.CreateDeliveriesForConstruction(this);
    }

    // runs 4 times per second
    IEnumerator QuarterUpdate()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        ConstructionCheck(); // how is our construction doing?
        ProductionCheck(); // can we produce?
        InfoBuilder(); // update our information
        StartCoroutine(QuarterUpdate()); // restart the update
    }

    // our production function
    void ProductionCheck()
    {
        // if we have enough of our required input material, spend it, wait the production time, then produce our output
        try
        {
            if (storedItems[inputType] >= inputRequirement && powerInput >= powerRequirement)
            {
                // spend that currency
                storedItems[inputType] -= inputRequirement;
                // perform production
                StartCoroutine(PerformProduction());
            }
        } catch { }
    }

    // a check we run to see how the construction of our building is going
    void ConstructionCheck()
    {
        bool hasItems = true; // do we have our items?

        foreach (var item in itemRequiredConstructionTypes)
        {
            // check to see if our stored items dictionary has enough of each item
            if (storedItems.ContainsKey(item))
            {
                if (storedItems[item] >= itemRequiredConstructionCounts[itemRequiredConstructionTypes.IndexOf(item)])
                    hasItems = false;
            }
        }

        // if we dont have any items, return
        if (!hasItems) return;

        // if we have all of our items
        Debug.Log(name + " is ready for construction labor");
    }

    // where our actual production is yielded
    IEnumerator PerformProduction()
    {
        yield return new WaitForSecondsRealtime(productionTime);

        // make sure we can yield this production
        float total = 0;

        foreach (var itemCount in storedItems.Values)
        {
            total += itemCount;
        }

        if (total + outputAmount <= storageMaximum)
            storedItems[outputType] += outputAmount;
        else
        {
            storedItems[outputType] += outputAmount;
            // then reduce to cap
            if (storedItems[outputType] + total > storageMaximum)
                storedItems[outputType] -= storageMaximum - storedItems[outputType];
        }
    }

    // our building info builder
    public void InfoBuilder()
    {
        // reset the string
        buildingInfo = "";

        // has our building been constructed yet?
        if (!buildingConstructed)
        {
            buildingInfo = "Building under construction..." + "\n";
            // then add in our needs
            foreach (var key in itemRequiredConstructionTypes)
            {
                buildingInfo += "Needs " + itemRequiredConstructionCounts[itemRequiredConstructionTypes.IndexOf(key)].ToString() + " " + key + "\n";
            }
        }

        // add in our storage information
        foreach (var key in storedItems.Keys)
        { 
            if (storedItems[key] > 0)
            {
                buildingInfo += "Storing " + key.ToString() + " " + storedItems[key].ToString() + "\n";
            }
        }

        // check our power requirement and consumption
        if (powerRequirement > 0)
            buildingInfo += "Power Requirement: " + powerRequirement + "\n";
        if (powerInput >= powerRequirement)
            buildingInfo += "Building is sufficiently powered."+ "\n";
        else
            buildingInfo += "Building is not powered." + "\n";
    }
}