using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Player player;

    public Animator playerAnimator;

    public void Initialize(Player playerRef)
    {
        player = playerRef;
    }

    public void UpdateAnimation()
    {
        playerAnimator.SetFloat("Speed", player.Velocity.magnitude);
    }
}
