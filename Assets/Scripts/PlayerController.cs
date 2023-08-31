using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// this script controls our player's camera and allows them to move throughout the space

    // the transform that we want to mimic. this moves jaggedly and roughly so that we may lerp to it.
    [SerializeField] Transform targetTransform;
    // our smoothed speeds for rotation and movement
    [SerializeField] float rotationSlerpSpeed; 
    [SerializeField] float movementLerpSpeed;
    [SerializeField] float targetKeyMovementSpeed; // our key movement sensitivity
    [SerializeField] float targetMouseMovementSpeed; // our mouse movement sensitivity
    [SerializeField] Vector3 inputVector; // our X/Z movement vector
    [SerializeField] Vector3 previousFrameMousePos; // the mouse position in the previous frame

    // our fixed update, runs 120 times per second
    private void FixedUpdate()
    {
        // process our camera movement so that it moves slowly and consistently
        ProcessCameraMovement();
    }

    private void Update()
    {
        // process our inputs so that we always move properly
        ProcessInputs();
    }

    // our camera processing function, runs every fixed update for movement
    void ProcessCameraMovement()
    {

    }

    // process our inputs from the player in the update functions
    void ProcessInputs()
    {
        // move our input vector using the WASD keys
        inputVector += new Vector3(Input.GetAxis("Horizontal") * targetKeyMovementSpeed, 0, Input.GetAxis("Vertical") * targetKeyMovementSpeed);
        // calculate the mouse movement distance and move our input vector by clicking and dragging with the middle mouse button
        if (Input.GetMouseButton(2))
            inputVector += (new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - previousFrameMousePos) * targetMouseMovementSpeed;
        // set our new mouse position
        previousFrameMousePos = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
        // set the position of our targetTransform
        targetTransform.position = inputVector;
        // calculate our rotation, when we press R add 45 degrees of rotation to our Y axis
        if (Input.GetKeyDown(KeyCode.R))
            targetTransform.eulerAngles = new Vector3(targetTransform.eulerAngles.x, targetTransform.eulerAngles.y + 45, targetTransform.eulerAngles.z);
    }
}