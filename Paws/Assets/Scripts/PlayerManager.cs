using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerController playerController;

    public bool isInteracting;

    private void Awake() {
        animator = GetComponent<Animator>();
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
        isInteracting = animator.GetBool("isInteracting");
        playerController.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerController.isGrounded);
    }
}
