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

            InventoryUI.Instance.Initialize();

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

    public void AddToInventory(string itemUID, int amount)
    {
        Debug.Log("Adding " + amount + " of " + itemUID + " to inventory.");

        // First try to fill existing stacks
        ItemInstance existingItemInstance = inventory.FirstOrDefault(i => i.itemReferenceUID == itemUID && i.amount < i.itemReference.maxStack);

        if (existingItemInstance != null)
        {
            int maxStack = existingItemInstance.itemReference.maxStack;
            int spaceInStack = maxStack - existingItemInstance.amount;
            
            if (amount <= spaceInStack)
            {
                // Can fit entirely in this stack
                existingItemInstance.amount += amount;
            }
            else
            {
                // Fill this stack and create new one(s) for remainder
                existingItemInstance.amount = maxStack;
                int remaining = amount - spaceInStack;
            
                // Create new stack(s) instead of recursive call
                while (remaining > 0)
                {
                    int stackAmount = Mathf.Min(remaining, maxStack);
                    ItemInstance newItemInstance = new ItemInstance(itemUID, stackAmount, -1);
                    inventory.Add(newItemInstance);
                    remaining -= stackAmount;
                }
            }
        }
        else
        {
            // No existing stack, create new stack(s)
            Item itemRef = ItemDatabase.Instance.GetItem(itemUID);
            int maxStack = itemRef.maxStack;
            
            while (amount > 0)
            {
                int stackAmount = Mathf.Min(amount, maxStack);
                ItemInstance newItemInstance = new ItemInstance(itemUID, stackAmount, -1);
                inventory.Add(newItemInstance);
                amount -= stackAmount;
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
                    // Use the item's inventorySlotIndex instead of SyncList index
                    InventoryUI.Instance.UpdateItem(itemInstance.inventorySlotIndex, itemInstance);
                    break;
                    
                case SyncList<ItemInstance>.Operation.OP_REMOVEAT:
                    // We need to find the UI slot index for the removed item
                    if (index < inventory.Count)
                    {
                        InventoryUI.Instance.RemoveItem(inventory[index].inventorySlotIndex);
                    }
                    break;
                    
                case SyncList<ItemInstance>.Operation.OP_INSERT:
                    InventoryUI.Instance.InsertItem(itemInstance.inventorySlotIndex, itemInstance);
                    break;

                case SyncList<ItemInstance>.Operation.OP_CLEAR:
                    InventoryUI.Instance.Initialize();
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