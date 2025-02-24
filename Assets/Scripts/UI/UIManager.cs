using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static Type CurrentUIState => Instance.currentState;
    public static bool IsPaused => Instance.currentState == typeof(PauseUI);
    public static bool IsInventoryOpen => Instance.currentState == typeof(InventoryUI);
    private Type currentState;

    public List<UISubclass> uiStates;
    public GameUI gameUI;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SwitchToState(null);
    }

    void Update()
    {
        foreach (UISubclass uiState in uiStates)
        {
            if (Input.GetKeyDown(uiState.toggleKey))
            {
                Type newState = uiState.GetType();

                if (currentState == newState)
                {
                    currentState = null;
                    SwitchToState(null);
                }
                else
                {
                    if (currentState == typeof(PauseUI))
                    {
                        return;
                    }

                    currentState = newState;
                    SwitchToState(newState);
                }
            }
        }
    }

    private void SwitchToState(Type state)
    {
        foreach (UISubclass uiState in uiStates)
        {
            if (uiState.GetType() == state)
            {
                Debug.Log($"Enabling {uiState.GetType()}");
                uiState.EnableUI();
            }
            else
            {
                Debug.Log($"Disabling {uiState.GetType()}");
                uiState.DisableUI();
            }
        }

        if (state == null)
        {
            gameUI.EnableUI();
        }
        else
        {
            gameUI.DisableUI();
        }
    }
}