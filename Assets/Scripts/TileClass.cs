using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles
    bool isVisible; // is this tile visible?
    bool isMouseOver; // is the mouse over?

    private void OnMouseEnter()
    {
        Debug.Log(gameObject.name + " is being hovered over");
        isMouseOver = true;
        // set this to our active tile
        PlayerMouse.instance.highlightedTile = this;
    }
    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    // make sure we know whether or not we are visible or invisible
    private void OnBecameVisible() => isVisible = true;
    private void OnBecameInvisible() => isVisible = false;
}
