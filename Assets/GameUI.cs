using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("Tabs")]
    private bool tabsActive = false;
    public GameObject darkenBackground;
    public GameObject inventoryUI;

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

    public void ToggleTabs()
    {
        tabsActive = !tabsActive;
        if (tabsActive)
        {
            EnableTabs();
        }
        else
        {
            DisableTabs();
        }
    }

    public void EnableTabs()
    {
        darkenBackground.SetActive(true);
        inventoryUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableTabs()
    {
        darkenBackground.SetActive(false);
        inventoryUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
