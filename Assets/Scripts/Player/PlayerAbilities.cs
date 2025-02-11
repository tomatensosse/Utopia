using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Properties;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public List<PlayerAbilityMode> abilityModes = new List<PlayerAbilityMode>();
    public List<PlayerAbilityType> abilitiesInInventory = new List<PlayerAbilityType>();
    public PlayerAbilityMode currentMode;
    private Player player;

    public void Initialize(Player playerRef)
    {
        player = playerRef;
        abilityModes = PlayerAbilityMode.GetDefaultModes();
        string set = abilityModes[0].uid;

        foreach (Item item in player.inventory)
        {
            foreach (PlayerAbilityType ability in item.abilities)
            {
                abilitiesInInventory.Add(ability);
            }
        }

        SetMode(set);
    }

    [Button("Set Mode")]
    public void SetMode(string uid)
    {
        foreach (PlayerAbilityMode mode in abilityModes)
        {
            if (mode.uid == uid)
            {
                currentMode = mode;
                Debug.Log("Player ability mode set to: " + uid);

                foreach (PlayerAbilityType ability in abilitiesInInventory)
                {
                    Type abilityType = ability.GetType();

                    bool enable = false;

                    foreach (Type type in mode.abilitiesToEnable)
                    {
                        if (abilityType.IsSubclassOf(type))
                        {
                            enable = true;
                            break;
                        }
                    }   

                    if (enable)
                    {
                        ability.Enable(player);
                    }
                    else
                    {
                        ability.Disable();
                    }
                }

                return;
            }
        }

        Debug.LogWarning("Player ability mode not found: " + uid);

        Debug.Log("Available modes:");
        string s = "";
        foreach (PlayerAbilityMode mode in abilityModes)
        {
            s+= mode.uid + ", ";
        }

        Debug.Log(s);
    }
}