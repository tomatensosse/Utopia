using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{

    private const int GRID_WIDTH = 8;
    private const int GRID_HEIGHT = 4;
    private const int BUTTON_SIZE = 50;
    private const int LABEL_FONT_SIZE = 20;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Inventory inventory = (Inventory)target;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = LABEL_FONT_SIZE,
            wordWrap = true
        };

        GUILayout.Label("Bora155's Inventory Script", labelStyle);
        GUILayout.Space(10);

        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Inventory Grid", labelStyle);
        GUILayout.Space(10);

        // display the inventory grid
        for (int y = 1; y < GRID_HEIGHT; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                int index = y * GRID_WIDTH + x;
                if (index < inventory.inventory.Length)
                {
                    GUILayout.BeginVertical(GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE));

                    Item item = ItemDatabase.GetItemByID(inventory.inventory[index].itemID);

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
                        Debug.Log("Clicked on " + item.ItemName);
                    }

                    // Optional: Display item name below the icon
                    if (item != null)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        string itemAmount = inventory.inventory[index].itemAmount.ToString();
                        string itemAmountDisplay = EditorGUILayout.TextField(itemAmount, GUILayout.Width(BUTTON_SIZE));
                        EditorGUI.EndDisabledGroup();
                    }

                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        GUILayout.Label("Hotbar Grid", labelStyle);
        GUILayout.Space(10);

        // display the hotbar grid

        GUILayout.BeginHorizontal();
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            int index = x;
            if (index < inventory.inventory.Length)
            {
                GUILayout.BeginVertical(GUILayout.Width(BUTTON_SIZE), GUILayout.Height(BUTTON_SIZE));

                Item item = ItemDatabase.GetItemByID(inventory.inventory[index].itemID);

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
                    Debug.Log("Clicked on " + item.ItemName);
                }

                // Optional: Display item name below the icon
                if (item != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    string itemAmount = inventory.inventory[index].itemAmount.ToString();
                    string itemAmountDisplay = EditorGUILayout.TextField(itemAmount, GUILayout.Width(BUTTON_SIZE));
                    if (itemAmountDisplay != item.ItemName)
                    EditorGUI.EndDisabledGroup();
                }

                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Load Inventory"))
        {
            inventory.LoadInventory();
        }

        if (GUILayout.Button("Save Inventory"))
        {
            inventory.SaveInventory();
        }

        if (GUILayout.Button("Demo Add Item"))
        {
            inventory.DemoAddItem();
        }

        if (GUILayout.Button("Print Items"))
        {
            inventory.DebugPrintItems();
        }
    }
}
#endif
