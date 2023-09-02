using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    /// class holds all the info for tiles
    bool isVisible; // is this tile visible?
    bool isMouseOver; // is the mouse over?

    // our fixedupdate
    private void FixedUpdate()
    {
        //ProcessHoverCheck();
    }

    // our check to see if we are being hovered
    void ProcessHoverCheck()
    {
        if (isMouseOver)
        {
            Debug.Log(gameObject.name + " is being hovered over");
        }
    }

    private void OnMouseOver() => isMouseOver = true;
    private void OnMouseExit() => isMouseOver = false;

    // make sure we know whether or not we are visible or invisible
    private void OnBecameVisible() => isVisible = true;
    private void OnBecameInvisible() => isVisible = false;
}
