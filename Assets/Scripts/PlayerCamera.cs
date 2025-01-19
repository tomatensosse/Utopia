using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance { get; private set; }

    public Camera mainCamera;
    public Transform cameraContainer;

    private Player targetPlayer;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Initialize(Player player)
    {
        targetPlayer = player;
        player.playerCamera = this;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateCamera(float xRotation, float yRotation)
    {
        cameraContainer.position = targetPlayer.cameraPosition.position;

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        cameraContainer.localRotation = Quaternion.Euler(0, yRotation, 0);
        targetPlayer.movement.orientation.rotation = cameraContainer.rotation;
    }
}
