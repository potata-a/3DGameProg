using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTransform;
    public Transform cameraPivot;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 1.0f;
    public float cameraPivotSpeed = 2.0f;
    public float lookAngle;
    public float pivotAngle;

    private void Awake(){
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void HandleAllCameraMovement(){
        FollowTarget();
        RotateCamera();
    }

    private void FollowTarget(){
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera(){
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, -35, 35);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions(){

    }
}
