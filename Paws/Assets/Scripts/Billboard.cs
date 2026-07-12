/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 11 July 2026
*/
using UnityEngine;

// Keeps a 2D sprite facing the main camera so fish pickups are readable from any
// angle in the 3D world.
public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main.transform;
        }

        // Face the camera (flip forward so the sprite front points at the viewer).
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
