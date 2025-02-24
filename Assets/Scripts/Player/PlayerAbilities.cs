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
    public List<PlayerAbilityType> activeAbilities = new List<PlayerAbilityType>();
    public PlayerAbilityMode currentMode;
    private Player player;

    public void Initialize(Player playerRef)
    {
        player = playerRef;
        abilityModes = PlayerAbilityMode.GetDefaultModes();
        string set = abilityModes[0].uid;

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

                foreach (PlayerAbilityType ability in activeAbilities)
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

    private void UpdateAbilitiesInCurrentMode()
    {
        foreach (PlayerAbilityType ability in activeAbilities)
        {
            Type abilityType = ability.GetType();

            bool enable = false;

            foreach (Type type in currentMode.abilitiesToEnable)
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
                Debug.Log("Enabling ability: " + abilityType.Name);
            }
            else
            {
                ability.Disable();
                Debug.Log("Disabling ability: " + abilityType.Name);
            }
        }
    }

    public void AddAbilitiesFromItem(Item item)
    {
        foreach (PlayerAbilityType ability in item.abilities)
        {
            activeAbilities.Add(ability);
        }

        UpdateAbilitiesInCurrentMode();
    }

    public void RemoveAbilitiesFromItem(Item item)
    {
        foreach (PlayerAbilityType ability in item.abilities)
        {
            activeAbilities.Remove(ability);
        }
        UpdateAbilitiesInCurrentMode();
    }
}