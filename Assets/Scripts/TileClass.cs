using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles

    [Header("Tile Information")]
    public string tileName; // our tile's name, ex: Grass Tile, Desert Tile
    public bool tileScanned; // has this tile been scanned?

    private void OnMouseEnter()
    {
        // set this to our active tile
        PlayerMouse.instance.highlightedTile = this;
    }
}
