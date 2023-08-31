using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// this script controls our player's camera and allows them to move throughout the space

    // the transform that we want to mimic. this moves jaggedly and roughly so that we may lerp to it.
    [SerializeField] Transform mimicTransform, targetTransform;
    // our containers which serve multiple functions to align the camera position
    [SerializeField] Transform cameraContainer, cameraZPosContainer; // the container that holds our camera
    // our smoothed speeds for rotation and movement
    [SerializeField] float rotationSlerpSpeed; 
    [SerializeField] float movementLerpSpeed;
    [SerializeField] float targetKeyMovementSpeed; // our key movement sensitivity
    [SerializeField] float targetMouseMovementSpeed; // our mouse movement sensitivity
    [SerializeField] float targetMouseScrollSensitivity; // our mouse scroll sensitivity
    [SerializeField] Vector3 inputVector; // our X/Z movement vector
    [SerializeField] Vector3 previousFrameMousePos; // the mouse position in the previous frame
    // our constraints
    [SerializeField] float zPosMin, zPosMax; // our minimum and maximum for our cameraZPosContainer

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
        // lerp our camera container to the positions and rotation of our target transform
        cameraContainer.position = Vector3.Lerp(cameraContainer.position, targetTransform.position, movementLerpSpeed * Time.fixedDeltaTime);
        cameraContainer.rotation = Quaternion.Lerp(cameraContainer.rotation, targetTransform.rotation, rotationSlerpSpeed * Time.fixedDeltaTime);
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
        // set final input to 0
        Vector3 finalInput = Vector3.zero;
        // if we have an input, calculate finalInput
        if (inputVector.x > 0 || inputVector.z > 0)
            finalInput = new Vector3(inputVector.x * mimicTransform.forward.x, inputVector.y * mimicTransform.forward.y, inputVector.z * mimicTransform.forward.z);

        // set the position of our targetTransform locally, after it has been calculated and rotated
        mimicTransform.localPosition = finalInput;

        // calculate our rotation, when we press R add 90 degrees of rotation to our Y axis
        if (Input.GetKeyDown(KeyCode.R))
            mimicTransform.localRotation = Quaternion.Euler(mimicTransform.localEulerAngles.x, mimicTransform.localEulerAngles.y + 90, mimicTransform.localEulerAngles.z);

        // zoom in and out with the camera by adding the scroll of the mouse to it
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f && cameraZPosContainer.localPosition.z + Input.mouseScrollDelta.y > zPosMin && cameraZPosContainer.localPosition.z + Input.mouseScrollDelta.y < zPosMax)
            cameraZPosContainer.localPosition += new Vector3(0,0,Input.mouseScrollDelta.y * targetMouseScrollSensitivity);
        // check to ensure we are not out of camera zPos bounds
        if (cameraZPosContainer.localPosition.z <= zPosMin)
            cameraZPosContainer.localPosition = new Vector3(cameraZPosContainer.localPosition.x, cameraZPosContainer.localPosition.y, zPosMin);
        if (cameraZPosContainer.localPosition.z >= zPosMax)
            cameraZPosContainer.localPosition = new Vector3(cameraZPosContainer.localPosition.x, cameraZPosContainer.localPosition.y, zPosMax);
    }
}