using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUIHandler : MonoBehaviour
{
    // this script handles our DEBUG ui on the screen which helps us to see how the game is running
    [SerializeField] GameObject debugUIParent; // the parent that we enable and disable
    [SerializeField] UnityEngine.UI.Text fixedUpdateDisplay; // the display of our fixedupdate

    private void Update()
    {
        ProcessUI();
    }

    void ProcessUI()
    {
        // to show and hide our UI when we press F12
        if (Input.GetKeyDown(KeyCode.F12))
            debugUIParent.SetActive(!debugUIParent.activeInHierarchy);
        // display our fixed update speed
        fixedUpdateDisplay.text = (Time.deltaTime / 60).ToString();
    }
}
