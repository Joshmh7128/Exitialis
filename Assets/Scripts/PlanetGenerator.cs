using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    // this script generates a whole planet for the game

    [SerializeField] float PlanetSize; // the size of our planet when generated

    // our tile types
    public enum TileTypes
    { rock, dirt, grass, ore, water }

    // the list of tile prefabs that we are going to use to instantiate our planet
    [SerializeField] List<GameObject> tilePrefabs = new List<GameObject>();

    // on start, generate our planet
    private void Start() => GeneratePlanet();

    // our generation function
    private void GeneratePlanet()
    {
        // create a grid of blocks on the X and Z axes
        for (int x = 0; x < PlanetSize; x++)
            for (int z = 0; z < PlanetSize; z++)
            {
                // get our y position as a combination of our x and z


                Instantiate(tilePrefabs[Random.Range(0,tilePrefabs.Count)], new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0,Random.Range((int)0,(int)5)*90f)), null);
            }
    }
}
