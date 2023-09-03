using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    /// this script is for all buildings in the game

    // storage
    public float storageMaximum; // how many items this building can hold in total

    // all of the possible things that can be stored in buildings, tracked
    public Dictionary<Itemtypes, float> storedItems = new Dictionary<Itemtypes, float>();

    // what can this building output?
    public enum Itemtypes
    {
        none, coal, ore, metal
    }

    // what does this building output?
    public Itemtypes outputType;

    // what does this building input?
    public Itemtypes inputType;

    // our production requirements
    public float inputRequirement, outputAmount, productionTime;

    // our production function
    void ProductionCheck()
    {
        // if we have enough of our required input material, spend it, wait the production time, then produce our output
        if (storedItems[inputType] >= inputRequirement)
        {
            // spend that currency
            storedItems[inputType] -= inputRequirement;
            // perform production
            StartCoroutine(PerformProduction());

        }
    }

    // where our actual production is yielded
    IEnumerator PerformProduction()
    {
        yield return new WaitForSecondsRealtime(productionTime);
        storedItems[outputType] += outputAmount;
    }

}
