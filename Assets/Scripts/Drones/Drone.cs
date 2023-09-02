using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Drone : MonoBehaviour
{
    /// this is the master script we are using to build all of our drones
    /// all drone functions run off of this one script, different drones will simply have different capabilities defined by this

    public string droneName; // the name of our drone, ex: Delivery Drone, Construction Drone

    // the states our drone can be in
    public enum DroneStates
    {
        recharge, delivery, construction, repair, explore, max
    }

    // the list of possible states that our drones can be in
    [SerializeField] List<DroneStates> possibleStates = new List<DroneStates>();
    public DroneStates currentState; // our current drone state
    int currentPossibleState; // our current possible state

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
    }

    // set our behaviour based on our current state
    void SetBehaviour()
    {
        // set our state
        currentState = possibleStates[currentPossibleState];
        // activate the state
        switch(currentState)
        {
            case DroneStates.recharge:
                break;
            case DroneStates.delivery:
                break;
            case DroneStates.construction:
                break;
            case DroneStates.repair:
                break;
            case DroneStates.explore:
                Debug.Log("Exploring...");
                StartCoroutine(ExploreBehaviour());
                break;
        }
    }

    // move to our next AI state
    void NextState()
    {
        // setting our next possible state
        if (currentPossibleState + 1 < possibleStates.Count)
            currentPossibleState++;
        else if (currentPossibleState + 1 >= possibleStates.Count)
            currentState = 0;

        // then set our current state
        currentState = possibleStates[currentPossibleState];
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
        // move on to the next behaviour
        NextState();
        // start the next state
        SetBehaviour();
        // end the coroutine
        yield break;
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
            progressSlider.value += Time.deltaTime * sliderProgressSpeed;
            // start again, run ever second
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(ProcessSliderVisual());
        }
        yield break;
    }

}
