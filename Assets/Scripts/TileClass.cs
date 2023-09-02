using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles

    [Header("Tile Information")]
    public string tileName; // our tile's name, ex: Grass Tile, Desert Tile
    public bool tileScanned, priorityScan; // has this tile been scanned?
    [SerializeField] MeshRenderer tileRenderer;

    private void Start()
    {
        CheckScan();
    }

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
            tileRenderer.material.color = Color.black;
    }

    // anything that happens on scan
    public void OnScan()
    {
        // when we become scanned set our material to white
        tileRenderer.material.color = Color.white;
    }
}
