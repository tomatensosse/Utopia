using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public ItemDatabase itemDatabase;
    public Inventory inventory;
    public GameObject inventorySlotItemPrefab;
    public InventorySlot[] inventorySlots;
    public GameObject[] inventorySlotItems;
    public GameObject mainInventoryUI;
    public KeyCode inventoryKey = KeyCode.I;
    public bool inventoryActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        itemDatabase = ItemDatabase.Instance;
    }

    public void Initialize(int inventorySize)
    {
        //inventorySlots = new InventorySlot[inventorySize];
        inventorySlotItems = new GameObject[inventorySize];
    }

    public void ToggleInventory()
    {
        inventoryActive = !inventoryActive;
        if (inventoryActive)
        {
            EnableInventory();
        }
        else
        {
            DisableInventory();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    private void EnableInventory()
    {
        mainInventoryUI.SetActive(true);
    }

    private void DisableInventory()
    {
        mainInventoryUI.SetActive(false);
    }

    public void GenerateItem(Item item, ItemData itemData, int slot)
    {
        if (item == null)
        {
            return;
        }

        if (slot < 0 || slot >= inventorySlots.Length)
        {
            Debug.LogError("Slot index out of range.");
            return;
        }

        if (inventorySlots[slot].transform.childCount > 0)
        {   
            Debug.LogError("Slot already has an item in it. Destroying item.");
            DestroyItem(slot);
        }

        GameObject generatedItem = Instantiate(inventorySlotItemPrefab, inventorySlots[slot].transform);
        inventorySlotItems[slot] = generatedItem;
        InventorySlotItem inventorySlotItem = generatedItem.GetComponent<InventorySlotItem>();

        inventorySlotItem.InitializeItem(item, itemData);
    }

    public void Load(ItemData[] itemDatas)
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            ItemData itemData = itemDatas[i];
            if (!string.IsNullOrEmpty(itemData.itemID))
            {
                Debug.Log("Loading item: " + itemData.itemID);
                Item item = itemDatabase.GetItemByID(itemData.itemID);
                GenerateItem(item, itemData, i);
            }
        }

        Debug.Log("Inventory loaded with UI.");
    }

    public void ChangeItemAmount(int slot, int amount)
    {
        InventorySlotItem inventorySlotItem = inventorySlotItems[slot].GetComponent<InventorySlotItem>();
        inventorySlotItem.itemData.itemAmount = amount;
        inventorySlotItem.RefreshCount();
    }

    public void ChangeItemDurability(int slot, int durability)
    {
        InventorySlotItem inventorySlotItem = inventorySlotItems[slot].GetComponent<InventorySlotItem>();
        inventorySlotItem.itemData.durability = durability;
        inventorySlotItem.RefreshDurability();
    }

    public void DestroyItem(int slot)
    {
        Destroy(inventorySlotItems[slot]);
        inventorySlotItems[slot] = null;
    }
}
