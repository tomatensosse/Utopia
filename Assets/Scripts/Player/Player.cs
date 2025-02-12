using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player LocalPlayer { get { return GameManager.Instance.localPlayer; } }

    [Header("Player Components")]
    public PlayerInteraction interaction;
    public PlayerMovement movement;
    public PlayerAbilities abilities;

    public readonly SyncList<Item> inventory = new SyncList<Item>();

    [Header("Camera Variables")]
    public Transform cameraPosition;
    public PlayerCamera playerCamera;
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    private float xRotation;
    private float yRotation;
    [SyncVar] private Vector3 orientationRotation;

    protected override void AuthorityInitialize()
    {
        base.AuthorityInitialize();
        GameManager.Instance.SetLocalPlayer(this);
    }

    protected override void ClientInitialize()
    {
        base.ClientInitialize();
        
        // Client-specific player setup
        if (isLocalPlayer)
        {
            inventory.OnChange += OnInventoryChanged;
            interaction.Initialize(this);
            movement.Initialize(this);
            abilities.Initialize(this);

            StartCoroutine(WaitForPlayerCamera());
        }
    }

    private IEnumerator WaitForPlayerCamera()
    {
        Debug.Log("Waiting for player camera...");
        yield return new WaitUntil(() => PlayerCamera.Instance != null);
        PlayerCamera.Instance.Initialize(this);
        Debug.Log("Player camera initialized!");
    }

    protected override void LocalUpdate()
    {
        base.LocalUpdate();

        if (IsOwnerOrSinglePlayer())
        {
            HandleInput();
            interaction.HandleInteraction();
            movement.UpdateMovement();
        }
    }

    protected override void LocalFixedUpdate()
    {
        base.LocalFixedUpdate();

        if (IsOwnerOrSinglePlayer())
        {
            movement.FixedUpdateMovement();
        }
    }

    private void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * mouseSensX;
        xRotation -= mouseY * mouseSensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        if (playerCamera != null)
        {
            playerCamera.UpdateCamera(xRotation, yRotation);
        }

        orientationRotation = movement.orientation.rotation.eulerAngles;
    }

    #region Inventory Management

    private void OnInventoryChanged(SyncList<Item>.Operation op, int index, Item item)
    {
        Debug.Log("Inventory changed: " + op + " | " + item.itemName);
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            inventory.OnChange -= OnInventoryChanged;
        }   
    }

    [Command]
    public void CmdAddItem(Item item)
    {
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
        }
    }

    [Command]
    public void CmdRemoveItem(Item item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    #endregion
}