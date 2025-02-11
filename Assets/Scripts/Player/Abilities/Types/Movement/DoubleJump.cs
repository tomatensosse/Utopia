using UnityEngine;

public class DoubleJump : MovementAbility
{
    public float jumpForce = 10f;
    private bool doubleJumped = false;

    public override void OnPlayerJump(bool isGroundedBeforeJump)
    {
        if (!isGroundedBeforeJump)
        {
            if (doubleJumped)
            {
                return;
            }

            Debug.Log($"Double Jump! Force: {jumpForce}");

            player.movement.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            doubleJumped = true;
        }
    }

    public override void OnPlayerLand()
    {
        doubleJumped = false;
    }
}