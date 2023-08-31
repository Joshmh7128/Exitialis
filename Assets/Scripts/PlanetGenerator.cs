using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    // this script generates a whole planet for the game

    [SerializeField] float PlanetSize; // the size of our planet when generated
    [SerializeField] float waveHeightContributor; // wave height contributor factor for our waves
    [SerializeField] int waveDistributionPercentage; // what is the chance that we change our wave?
    // our tile types
    public enum TileTypes
    { rock, dirt, grass, ore, water }

    // the list of tile prefabs that we are going to use to instantiate our planet
    [SerializeField] List<GameObject> tilePrefabs = new List<GameObject>();
    [SerializeField] List<int> tileWeights = new List<int>();

    // on start, generate our planet
    private void Start() => GeneratePlanet();

    // our generation function
    private void GeneratePlanet()
    {
        // let's create some grid data
        int[,] LookupGrid = new int[(int)PlanetSize,(int)PlanetSize];
        // randomly move throughout the grid, and place weighted numbers in it to assign regions to the space
        for (int lx = 0; lx < PlanetSize; lx++)
            for (int ly = 0; ly < PlanetSize; ly++)
            {
                /*int li = Random.Range(0, 100);
                if (li <= waveDistributionPercentage)*/
                    LookupGrid[lx,ly] = ChooseNumber();
            }


        // create a grid of blocks on the X and Z axes
        for (int x = 0; x < PlanetSize; x++)
            for (int z = 0; z < PlanetSize; z++)
            {
                // get our y position as a combination of our x and z
                float yPos = (Mathf.Sin(x) + Mathf.Cos(z)) * waveHeightContributor;

                // place a tile according to the lookup grid's information
                Instantiate(tilePrefabs[LookupGrid[x, z]], new Vector3(x, yPos, z), Quaternion.Euler(0, Random.Range(0, 5) * 90, 0));

             }
    }

    int ChooseNumber()
    {
        // get the total size of our weights
        int total = 0;
        foreach (int t in tileWeights)
            total += t;
        // choose a random number from our total weight pool
        int i = Random.Range(0, total);
        Debug.Log(total);

        // our total counting upwards
        int tx = 0;
        // then check against the pool
        for (int x = 0; x < tileWeights.Count; x++)
        {
            // make sure we check the 0th entry
            if (x == 0)
            {
                if (i < tx + tileWeights[x])
                    return x;
            }

            // check to see if it is between the current weight and the next weight
            if (x + 1 < tileWeights.Count)
            {
                // if we are checking anything but the first or last index...
                if (i > tx && i < tx + tileWeights[x] || i == tx || i == tileWeights[x])
                    return x;
            }
            // make sure we are checking for something that is there...
            if (x + 1 > tileWeights.Count)
            {
                if (i >= tx && i <= tx + tileWeights[x] + 1 || i == tx)
                    return x;
            }

            // if we havent found the number, add our current weight to tx
            tx += tileWeights[x];
        }

        // if we failed, return 5 for the red cube
        return 5;
    }
}
