using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : UISubclass
{
    public static GameUI Instance { get; private set; }

    public List<GameUI_Slot> hotbarSlots;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;   
    }

    public void Initialize()
    {
        foreach (GameUI_Slot hotbarSlot in hotbarSlots)
        {
            hotbarSlot.Initialize();
        }
    }
}
