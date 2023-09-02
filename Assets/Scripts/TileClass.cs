using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles
    private void OnMouseEnter()
    {
        Debug.Log(gameObject.name + " is being hovered over");
        // set this to our active tile
        PlayerMouse.instance.highlightedTile = this;
    }
}
