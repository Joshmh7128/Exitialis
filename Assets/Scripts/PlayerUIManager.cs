using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerUIManager : MonoBehaviour
{
    /// <summary>
    /// Script manages all of the UI elements on the screen, serves as a junction and instance for UI elements to communicate
    /// </summary>

    // our instance
    public static PlayerUIManager instance;
    void Awake() => instance = this;

    public List<GameObject> ActiveDynamicUIElements; // all active UI elements in the game

    // close all UI panels on the screen
    public void ClearDynamicUI()
    {
        // rebuild the list
        ActiveDynamicUIElements.RemoveAll(item => item == null);

        // manage our dynamic ui
        foreach (GameObject go in ActiveDynamicUIElements)
        {
            // check what kind of UI element this object is

            // if this is a tile info popup, destroy the object
            if (go.GetComponent<TileInfoPopup>())
            {
                if (go != null)
                Destroy(go);
            }
        }

        // rebuild the list
        ActiveDynamicUIElements.RemoveAll(item => item == null);

    }
}
