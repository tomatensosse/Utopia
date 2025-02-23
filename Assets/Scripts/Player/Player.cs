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

    public readonly SyncList<ItemInstance> inventory = new SyncList<ItemInstance>();

    [Header("Camera Variables")]
    public Transform cameraPosition;
    public PlayerCamera playerCamera;
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    private float xRotation;
    private float yRotation;
    [SyncVar] private Vector3 orientationRotation; // TBA

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

            InventoryUI.Instance.Initialize();

            StartCoroutine(WaitForPlayerCamera());
        }
    }

    private IEnumerator WaitForPlayerCamera()
    {
        //Debug.Log("Waiting for player camera...");
        yield return new WaitUntil(() => PlayerCamera.Instance != null);
        PlayerCamera.Instance.Initialize(this);
        //Debug.Log("Player camera initialized!");
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

    public void AddToInventory(ItemInstance itemInstance)
    {
        itemInstance.Deserialize();

        int amountRemaining = itemInstance.amount;

        Debug.Log($"Adding a total of {amountRemaining} {itemInstance.itemReference.name} to inventory.");

        while (amountRemaining > 0)
        {
            int index = inventory.FindIndex(i => i.itemReferenceUID == itemInstance.itemReferenceUID && i.amount < itemInstance.itemReference.maxStack);

            if (index >= 0)
            {
                ItemInstance existingItemInstance = inventory[index];

                Debug.Log($"ExistingItemInstance Old Amount = {existingItemInstance.amount}");

                int amountToAdd = Mathf.Min(itemInstance.itemReference.maxStack - existingItemInstance.amount, amountRemaining);
                int newAmount = existingItemInstance.amount + amountToAdd;
                existingItemInstance.amount = newAmount;
                amountRemaining -= amountToAdd;

                Debug.Log($"ExistingItemInstance New Amount = {existingItemInstance.amount}");

                inventory[index] = existingItemInstance; // Triggers OP_SET on Clients

                Debug.Log($"Inventory[index] ItemInstance Amount = {inventory[index].amount}");
            }
            else
            {
                int amountToAdd = Mathf.Min(itemInstance.itemReference.maxStack, amountRemaining);
                amountRemaining -= amountToAdd;

                ItemInstance newItemInstance = new ItemInstance(itemInstance.itemReferenceUID, amountToAdd);

                inventory.Add(newItemInstance);           
            }
        }
    }

    private void OnInventoryChanged(SyncList<ItemInstance>.Operation op, int index, ItemInstance itemInstance)
    {
        if (isLocalPlayer)
        {
            switch (op)
            {
                case SyncList<ItemInstance>.Operation.OP_ADD:
                    InventoryUI.Instance.AddNewItem(itemInstance);
                    break;
                    
                case SyncList<ItemInstance>.Operation.OP_SET:
                    InventoryUI.Instance.UpdateItem(inventory[index]); // Is this being passed the old itemInstance ?

                    Debug.Log($"OnInventoryChanged ItemInstance Amount = {itemInstance.amount}"); // Returns 2 (Which is the old amount)
                    Debug.Log($"OnInventoryChanged Inventory[index] ItemInstance Amount = {inventory[index].amount}"); // Returns 4 (Which is the new and correct amount)
                    break;
                    
                case SyncList<ItemInstance>.Operation.OP_REMOVEAT:
                    Debug.Log("OP_REMOVEAT");
                    break;
                    
                case SyncList<ItemInstance>.Operation.OP_INSERT:
                    Debug.Log("OP_INSERT");
                    break;

                case SyncList<ItemInstance>.Operation.OP_CLEAR:
                    Debug.Log("OP_CLEAR");
                    break;
            }
        }
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            inventory.OnChange -= OnInventoryChanged;
        }   
    }

    #endregion
}