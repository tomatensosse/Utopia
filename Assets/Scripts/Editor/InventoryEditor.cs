using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

#if UNITY_EDITOR
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{

    private const int GRID_WIDTH = 8;
    private const int GRID_HEIGHT = 4;
    private const int BUTTON_SIZE = 50;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Inventory inventory = target.GetComponent<Inventory>();

        GUILayout.Label("Inventory Grid", EditorStyles.boldLabel);

        bool hasChanged = false;

        // display the inventory grid
        for (int y = 1; y < GRID_HEIGHT; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                int index = y * GRID_WIDTH + x;
                if (index < inventory.inventory_itemDatas.Length)
                {
                    GUILayout.BeginVertical(GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE));

                    Item item = ItemDatabase.Instance.GetItemByID(inventory.inventory_itemDatas[index].itemID);

                    // Create a GUIContent with icon
                    GUIContent content = new GUIContent();
                    if (item != null && item.ItemIcon != null)
                    {
                        // Convert Sprite to Texture2D for display in Editor
                        Texture2D iconTexture = AssetPreview.GetAssetPreview(item.ItemIcon);
                        content.image = iconTexture;
                    }

                    // Display the icon
                    if (GUILayout.Button(content, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                    {
                        // Optional: Handle button click if needed
                        Debug.Log("Clicked on " + (item != null ? item.ItemName : "empty slot"));
                    }

                    // Optional: Display item name below the icon
                    if (item != null)
                    {
                        string itemAmount = inventory.inventory_itemDatas[index].itemAmount.ToString();
                        string itemAmountDisplay = EditorGUILayout.TextField(itemAmount, GUILayout.Width(BUTTON_SIZE));
                        if (itemAmountDisplay != item.ItemName)
                        {
                            item.ItemName = itemAmountDisplay;
                            hasChanged = true;
                        }
                    }

                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Label("Hotbar Grid", EditorStyles.boldLabel);

        // display the hotbar grid

        GUILayout.BeginHorizontal();
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            int index = x;
            if (index < inventory.inventory_itemDatas.Length)
            {
                GUILayout.BeginVertical(GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE));

                Item item = ItemDatabase.Instance.GetItemByID(inventory.inventory_itemDatas[index].itemID);

                // Create a GUIContent with icon
                GUIContent content = new GUIContent();
                if (item != null && item.ItemIcon != null)
                {
                    // Convert Sprite to Texture2D for display in Editor
                    Texture2D iconTexture = AssetPreview.GetAssetPreview(item.ItemIcon);
                    content.image = iconTexture;
                }

                // Display the icon
                if (GUILayout.Button(content, GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE)))
                {
                    // Optional: Handle button click if needed
                    Debug.Log("Clicked on " + (item != null ? item.ItemName : "empty slot"));
                }

                // Optional: Display item name below the icon
                if (item != null)
                {
                    string itemAmount = inventory.inventory_itemDatas[index].itemAmount.ToString();
                    string itemAmountDisplay = EditorGUILayout.TextField(itemAmount, GUILayout.Width(BUTTON_SIZE));
                    if (itemAmountDisplay != item.ItemName)
                    {
                        item.ItemName = itemAmountDisplay;
                        hasChanged = true;
                    }
                }

                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Add Random Item"))
        {
            inventory.Add();
        }

        if (GUILayout.Button("Save"))
        {
            inventory.Save();
        }

        if (GUILayout.Button("Load"))
        {
            inventory.Load();
        }

        if (GUILayout.Button("Craft"))
        {
            inventory.Craft();
        }
    }
}
#endif