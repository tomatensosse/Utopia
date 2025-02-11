using UnityEngine;

public class MovementAbility : PlayerAbilityType
{
    public override void Enable(Player playerRef)
    {
        base.Enable(playerRef);

        EnableMovementAbility();
    }

    public override void Disable()
    {
        base.Disable();

        DisableMovementAbility();
    }

    public virtual void EnableMovementAbility()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is null");
            return;
        }

        PlayerMovement playerMovement = player.movement;

        Debug.Log("Enabling Movement Ability");

        playerMovement.onPlayerJump += OnPlayerJump;
        playerMovement.onPlayerLand += OnPlayerLand;
        playerMovement.onPlayerSpacebar += OnPlayerSpacebar;

        Debug.Log("Subscribed to Player Movement Events");
    }

    public virtual void DisableMovementAbility()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is null");
            return;
        }

        PlayerMovement playerMovement = player.movement;

        Debug.Log("Disabling Movement Ability");

        playerMovement.onPlayerJump -= OnPlayerJump;
        playerMovement.onPlayerLand -= OnPlayerLand;
        playerMovement.onPlayerSpacebar -= OnPlayerSpacebar;

        Debug.Log("Unsubscribed to Player Movement Events");
    }

    public virtual void OnPlayerJump(bool isGroundedBeforeJump)
    {
        Debug.Log("Player Jumped");
    }

    public virtual void OnPlayerLand()
    {
        Debug.Log("Player Landed");
    }

    public virtual void OnPlayerSpacebar()
    {
        Debug.Log("Player Pressed Spacebar");
    }
}