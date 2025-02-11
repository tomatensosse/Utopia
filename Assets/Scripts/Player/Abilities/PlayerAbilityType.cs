using System;
using UnityEngine;

[Serializable]
public class PlayerAbilityType
{
    protected Player player;
    protected bool enabled = false;

    public virtual void Enable(Player playerRef)
    {
        if (enabled)
        {
            Debug.Log("Ability already enabled");
            return;
        }

        player = playerRef;
        enabled = true;

        Debug.Log("Enabling Ability | " + GetType().Name);
    }

    public virtual void Disable()
    {
        if (!enabled)
        {
            Debug.Log("Ability already disabled");
            return;
        }

        enabled = false;

        Debug.Log("Disabling Ability | " + GetType().Name);
    }
}