using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlanetGenerator : MonoBehaviour
{
    // this script generates a whole planet for the game

    public int PlanetSize; // the size of our planet when generated
    [SerializeField] float waveHeightContributor; // wave height contributor factor for our waves
    [SerializeField] float waveDistributionPercentage; // what is the chance that we change our wave?
    [SerializeField] int heuristicCheck; // how many times do we run the generation check?
    [SerializeField] bool canGen; // can we gen?

    int[,] GeneratedGrid; // our final generated planet grid in integer values
    public TileClass[,] PlanetTiles; // our final generated planet grid of actual tiles

    // our tile types
    public enum TileTypes
    { rock, dirt, grass, ore, water }

    // the list of tile prefabs that we are going to use to instantiate our planet
    [SerializeField] List<GameObject> tilePrefabs = new List<GameObject>();
    [SerializeField] List<int> tileWeights = new List<int>();

    // here is our list of special tiles that are added into the mix after the generation is complete, and before the tiles are instantiated
    [SerializeField] List<GameObject> specialTiles = new List<GameObject>();
    [SerializeField] List<int> specialTileAmounts = new List<int>();

    // our instance
    public static PlanetGenerator instance;
    private void Awake() => instance = this; // set our instance

    // on start, generate our planet
    private void Start()
    {
        if (canGen)
            GeneratePlanet();
    }

    // our generation function
    private void GeneratePlanet()
    {
        // make sure our planet grid is ready to go
        PlanetTiles = new TileClass[PlanetSize, PlanetSize];

        // let's create some grid data to reference what we are spawning in
        int[,] LookupGrid = new int[(int)PlanetSize, (int)PlanetSize];
        // the array representing our source tiles that are placed by the generator
        int[,] SourceTiles = new int[(int)PlanetSize, (int)PlanetSize];
        // randomly throughout the grid, place weighted numbers in it to assign regions to the space
        for (int lx = 0; lx < PlanetSize; lx++)
        {
            int ly = 0;
            while (ly < PlanetSize)
            {
                float li = Random.Range(0, 100);
                if (li <= waveDistributionPercentage)
                {
                    // set to a random source tile
                    LookupGrid[lx, ly] = ChooseSourceTile();
                    // mark it as a source
                    SourceTiles[lx, ly] = 1;
                }

                // add to y
                ly++;
            }
        }
       
        // run a breadth check with our lookup grid and source tiles
        BreadthCheck(LookupGrid, SourceTiles);

        // output our grid
        GeneratedGrid = new int[PlanetSize, PlanetSize];
        GeneratedGrid = LookupGrid;

        // add in special tiles
        SpecialTilePlacement();

        // spawn the planet
        for (int x = 0; x < PlanetSize; x++)
        {
            int y = 0;
            while (y < PlanetSize)
            {
                SpawnTile(x, y);
                y++;
            }
        }

        // once the planet is ready, bake our navigation map
        PlanetTiles[0, 0].gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    void BreadthCheck(int[,] LookupGrid, int[,] SourceTiles)
    {
        for (int x = 0; x < PlanetSize; x++)
        {
            int y = 0;
            while (y < PlanetSize)
            {
                // the closest x/y coord
                float closestDistance = PlanetSize; int closestValue = 0; // our closest value
                // if we are not set, find the nearest source tile within a radius to ourselves and copy it
                if (LookupGrid[x, y] == 0)
                {
                    /// perform a breadth first search
                    // our queue of coordinates to check
                    Queue<Coordinates> neighborQueue = new Queue<Coordinates>();
                    // which neighbors have we checked so far?
                    int[,] checkedGrid = new int[(int)PlanetSize, (int)PlanetSize];
                    // start with our center coordinate of where we are
                    List<Coordinates> neighborList = FindNeighbors(x, y, LookupGrid, checkedGrid);
                    // load up our neighbors
                    foreach (Coordinates neighbor in neighborList)
                    {
                        // enqueue the neighbor
                        neighborQueue.Enqueue(neighbor);
                        // set it to checked
                        checkedGrid[neighbor.x, neighbor.y] = 1;
                    }

                    // needs to be a function
                    NeighborCheck(x, y, closestDistance, closestValue, neighborQueue, SourceTiles, LookupGrid, checkedGrid, heuristicCheck);
                }

                // advance
                y++;
            }
        }
    }

    void NeighborCheck(int lx2, int ly2, float closestDistance, int closestValue, Queue<Coordinates> neighborQueue, int[,] SourceTiles, int[,] LookupGrid, int[,] checkedGrid, int iterations)
    {
        for (int j = 0; j < iterations; j++)
        {
            // run a neighbor check
            for (int i = 0; i < neighborQueue.Count; i++)
            {
                // check the coordinate
                Coordinates c = neighborQueue.Dequeue();
                // is this a source? is it closer to us than the closest distance?
                if (SourceTiles[c.x, c.y] == 1 && Vector2.Distance(new Vector2(c.x, c.y), new Vector2(lx2, ly2)) < closestDistance)
                {
                    // this is now our closest value
                    closestValue = LookupGrid[c.x, c.y];
                    // this is our closest distance
                    closestDistance = Vector2.Distance(new Vector2(c.x, c.y), new Vector2(lx2, ly2));

                }
                else // if this is not a source
                if (SourceTiles[c.x, c.y] == 0)
                {
                    // add its neighbors to the queue
                    List<Coordinates> localList = FindNeighbors(c.x, c.y, LookupGrid, checkedGrid);
                    foreach (Coordinates local in localList)
                    {
                        // set it to be checked
                        neighborQueue.Enqueue(local);
                        // set it to checked
                        checkedGrid[local.x, local.y] = 1;
                    }
                }
            }

            // if we can, set the thing
            if (closestValue != 0)
            {
                LookupGrid[lx2, ly2] = closestValue;
            }

            // if we have an error...
            if (closestValue == 0)
            {

                float cd = PlanetSize; int cv = 0;
                /// find the nearest non 0 value and use it to fill in the gaps
                for (int x = 0; x < PlanetSize; x++)
                {
                    int y = 0;
                    while (y < PlanetSize)
                    {
                        // if the tile we are checking is not 0 and the distance from us to this tile is smaller...
                        if (LookupGrid[x, y] != 0 && Vector2.Distance(new Vector2(x, y), new Vector2(lx2, ly2)) < cd)
                        {
                            cd = Vector2.Distance(new Vector2(x, y), new Vector2(lx2, ly2));
                            cv = LookupGrid[x, y];
                        }

                        y++;
                    }
                }
                LookupGrid[lx2, ly2] = cv;
            }

        }

    }

    // select a source tile to work from
    int ChooseSourceTile()
    {
        // get the total size of our weights
        int total = 0;
        foreach (int t in tileWeights)
            total += t;
        // choose a random number from our total weight pool
        int i = Random.Range(0, total);

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
        return 0;
    }

    // special tile construction
    void SpecialTilePlacement()
    {
        // our first two specials are always going to be the starter building and drone
        // place our starter building in the center, then our drone next to it
        GeneratedGrid[PlanetSize / 2, PlanetSize / 2] = tilePrefabs.Count; // this will be the 0th special tile
        GeneratedGrid[PlanetSize / 2 + 1, PlanetSize / 2] = tilePrefabs.Count+1; // this will be the 1st special tile
    }

    // checks for our neighbors:            up,      down,      left,     right
    int[,] NeighborChecks = new int[,] { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } };

    // find and return coordinates
    List<Coordinates> FindNeighbors(int inpx, int inpy, int[,] grid, int[,] checkedGrid)
    {
        // new list
        List<Coordinates> neighbors = new List<Coordinates>();
        // check all coordinates around the tile
        for (int i = 0; i < 4; i++)
        {
            // add the coordinate to the neighbors
            Coordinates coords = new Coordinates(); 
            coords.x = NeighborChecks[i,0] + inpx;
            coords.y = NeighborChecks[i, 1] + inpy;

            // check to make sure that the coordinates exist, then add
            try
            {
                if (grid[coords.x, coords.y] != 0 && checkedGrid[coords.x, coords.y] != 1)
                {
                    neighbors.Add(coords);
                }
            } catch { }
        }

        // then return our neighbors
        return neighbors;
    }

    // frametime generation tracking
    int x, y;

    // spawning tiles one at a time
    void FrametimeTileSpawning()
    {
        SpawnTile(x, y);
        x++; 
        if (x >= PlanetSize)
        {
            x = 0;
            y++;
        }
    }

    // spawn in tiles based off of information
    void SpawnTile(int x, int z)
    {
        if (GeneratedGrid[x, z] < tilePrefabs.Count)
        {
            // place a tile according to the lookup grid's information
            TileClass tile = Instantiate(tilePrefabs[GeneratedGrid[x, z]], new Vector3(transform.position.x + x, 0, transform.position.z + z), Quaternion.Euler(0, Random.Range(0, 5) * 90, 0), transform).GetComponent<TileClass>();
            // add that to our final reference grid of tiles
            PlanetTiles[x, z] = tile;
        }
        else
        {
            // place a tile according to the lookup grid's information
            TileClass tile = Instantiate(specialTiles[tilePrefabs.Count+1-GeneratedGrid[x, z]], new Vector3(transform.position.x + x, 0, transform.position.z + z), Quaternion.Euler(0, Random.Range(0, 5) * 90, 0), transform).GetComponent<TileClass>();
            // add that to our final reference grid of tiles
            PlanetTiles[x, z] = tile;
        }
    }

}

// for our grid search operations
public class Coordinates
{
    // our x and y coordinates
    public int x, y;
}