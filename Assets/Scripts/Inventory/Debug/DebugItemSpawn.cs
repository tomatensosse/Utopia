using System.Collections.Generic;
using UnityEngine;

public class DebugItemSpawn : MonoBehaviour
{
    [Header("In")]
    public string itemID;
    public int itemAmount;
    public int durability;
    public string savePath = "Assets/Scripts/Inventory/Debug/InventorySave.json";
    public List<Item> itemsForDemo;
    public int inventorySize = 6;
    public ItemData[] inventory_itemDatas;
    public int selectedSlot = 0;

    private void Start()
    {
        GenerateDemoInventory();
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            ChangeSelectedSlot(Input.inputString);
        }
    }

    private void ChangeSelectedSlot(string input)
    {
        if (int.TryParse(input, out int result))
        {
            selectedSlot = result;
        }
    }

    public void Spawn()
    {
        ItemData itemData = new ItemData().Generate(itemID, itemAmount, durability);
        Debug.Log(itemData.itemID);
        Debug.Log(itemData.itemAmount);
        Debug.Log(itemData.durability);
        string json = itemData.ItemToJson();
        Debug.Log(json);
    }

    private void GenerateDemoInventory()
    {
        inventory_itemDatas = new ItemData[inventorySize];

        for (int i = 0; i < inventory_itemDatas.Length; i++)
        {
            if (i < itemsForDemo.Count)
            {
                Item item = itemsForDemo[i];
                inventory_itemDatas[i] = new ItemData().Generate(item.ItemID, Random.Range(1, 10), Random.Range(1, 100));
            }
            else
            {
                inventory_itemDatas[i] = null;
            }
        }
    }

    private bool AddItemToInventory(Item item, int amount = 1, int durability = -1)
    {
        ItemData itemSerialized = new ItemData().Generate(item.ItemID, amount, durability);

        return false;
    }

    public void SaveInventory()
    {
        // Create a new list of itemDatas
        inventory_itemDatas = new ItemData[inventorySize];

        foreach (ItemData itemData in inventory_itemDatas)
        {
            
        }
    }

    public void LoadInventory()
    {

    }
}
