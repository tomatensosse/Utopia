using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilityMode
{
    public string uid;
    public string modeName;
    public List<Type> abilitiesToEnable = new List<Type>();

    public void AddAbilityType<T>() where T : PlayerAbilityType
    {
        abilitiesToEnable.Add(typeof(T));
    }

    public static List<PlayerAbilityMode> GetDefaultModes()
    {
        List<PlayerAbilityMode> defaultModes = new List<PlayerAbilityMode>();

        PlayerAbilityMode inactive = new PlayerAbilityMode("INACTIVE");
        defaultModes.Add(inactive);

        PlayerAbilityMode active = new PlayerAbilityMode("ACTIVE");
        active.AddAbilityType<CombatAbility>();
        active.AddAbilityType<MovementAbility>();
        defaultModes.Add(active);

        PlayerAbilityMode mining = new PlayerAbilityMode("MINING");
        mining.AddAbilityType<MiningAbility>();
        defaultModes.Add(mining);

        return defaultModes;
    }

    public PlayerAbilityMode(string modeName)
    {
        this.modeName = modeName;
        this.uid = modeName.ToLower().ToSafeString();
    }
}