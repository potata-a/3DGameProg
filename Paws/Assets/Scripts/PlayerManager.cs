using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerController playerController;

    private void Awake() {
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update() {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate(){
        playerController.HandleAllMovement();
    }

    private void LateUpdate() {
        cameraManager.HandleAllCameraMovement();
    }
}
