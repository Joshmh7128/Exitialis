using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    private void Awake() => instance = this;
    /// this script exists to manage all of our buildings
    public List<GameObject> buildingPrefabs; // all of our buildings
    // building enum
    public enum BuildingTypes
    {
        test
    }

    // all of our buildings
    public List<Building> buildings = new List<Building>();

}
