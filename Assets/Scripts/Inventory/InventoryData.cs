using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class InventoryData
{
    public int inventorySize = 32;
    public ItemData[] inventory;

    public string InventoryToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static InventoryData JsonToInventory(string json)
    {
        return JsonConvert.DeserializeObject<InventoryData>(json);
    }

    public void SaveToFile(string path)
    {
        string json = InventoryToJson();
        System.IO.File.WriteAllText(path, json);
    }

    public static InventoryData LoadFromFile(string path)
    {
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonToInventory(json);
        }
        return new InventoryData();
    }
}
