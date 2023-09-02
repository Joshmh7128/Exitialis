using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanRequestClass : MonoBehaviour
{
    public TileClass parentClass;
    // for our scan request dynamic UI element
    public UnityEngine.UI.Toggle toggle;

    bool isMouseOver; // is the mouse over?

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void Update()
    {
        // check if we are clicked
        if (isMouseOver && Input.GetMouseButtonDown(0))
            toggle.isOn = !toggle.isOn;
    }
    
    // for the onchange event of our toggle
    public void SetPriorityScan()
    {
        parentClass.priorityScan = toggle.isOn;
    }

}
