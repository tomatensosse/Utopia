using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    public enum Tab { None, Inventory, Crafting, Map }
    public static GameManagerUI Instance { get; private set; }

    public Tab activeTab;
    public GameObject inventoryTab, craftingTab, mapTab;
    public KeyCode inventoryKey, craftingKey, mapKey;
    public GameObject tabsButtons;
    public GameObject hotbar;
    public GameObject darkBackground;
    public bool anyTabOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManagerUI found!");
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        #region Input
        if (Input.GetKeyDown(inventoryKey))
        {
            if (activeTab == Tab.Inventory)
            {
                CloseTabs();
            }
            else
            {
                OpenTabs("Inventory");
            }
        }
        else if (Input.GetKeyDown(craftingKey))
        {
            if (activeTab == Tab.Crafting)
            {
                CloseTabs();
            }
            else
            {
                OpenTabs("Crafting");
            }
        }
        else if (Input.GetKeyDown(mapKey))
        {
            if (activeTab == Tab.Map)
            {
                CloseTabs();
            }
            else
            {
                OpenTabs("Map");
            }
        }
        #endregion
    }   

    public void Init()
    {
        anyTabOpen = false;

        DisableAllTabs();
    }

    public void OpenTabs(string tab)
    {
        if (!anyTabOpen)
        {
            darkBackground.SetActive(true);
            anyTabOpen = true;
            
            tabsButtons.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            anyTabOpen = true;
        }

        ChangeTab(tab);
    }

    public void CloseTabs()
    {
        darkBackground.SetActive(false);
        anyTabOpen = false;

        tabsButtons.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DisableAllTabs();
    }

    public void DisableAllTabs()
    {
        inventoryTab.SetActive(false);
        craftingTab.SetActive(false);
        mapTab.SetActive(false);

        tabsButtons.SetActive(false);

        hotbar.SetActive(true);

        darkBackground.SetActive(false);

        anyTabOpen = false;

        activeTab = Tab.None;
    }

    public void SwitchToTab(Tab tab)
    {
        DisableTab(activeTab);

        switch (tab)
        {
            case Tab.Inventory:
                inventoryTab.SetActive(true);
                hotbar.SetActive(true);
                activeTab = Tab.Inventory;
                break;
            case Tab.Crafting:
                craftingTab.SetActive(true);
                hotbar.SetActive(false);
                activeTab = Tab.Crafting;
                break;
            case Tab.Map:
                mapTab.SetActive(true);
                hotbar.SetActive(false);
                activeTab = Tab.Map;
                break;
            default:
                break;
        }

        activeTab = tab;
    }

    public void DisableTab(Tab tab)
    {
        switch (tab)
        {
            case Tab.Inventory:
                inventoryTab.SetActive(false);
                break;
            case Tab.Crafting:
                craftingTab.SetActive(false);
                break;
            case Tab.Map:
                mapTab.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void ChangeTab(string tab) // Used for buttons and shit
    {
        switch (tab)
        {
            case "Inventory":
                SwitchToTab(Tab.Inventory);
                break;
            case "Crafting":
                SwitchToTab(Tab.Crafting);
                break;
            case "Map":
                SwitchToTab(Tab.Map);
                break;
            default:
                Debug.LogWarning("Ece Alpagut");
                break;
        }

        anyTabOpen = true;
    }
}
