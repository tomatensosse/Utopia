using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator playerBodyAnimator;
    public Animator playerHandRightAnimator, playerHandLeftAnimator;

    [Header("Animation Tresholds")]
    public float _runAnimationParam; // readonly
    public float runAnimationTreshold = 5f;
    public float runAnimationLerp = 0.05f;

    public void Update()
    {
        AnimRun();
    }

    public void TriggerWield()
    {
        playerHandRightAnimator.SetTrigger("wield");
    }

    private void SetSpeed(float speed)
    {
        playerBodyAnimator.SetFloat("speed", Mathf.Lerp(playerBodyAnimator.GetFloat("speed"), speed, runAnimationLerp));
        playerHandLeftAnimator.SetFloat("speed", Mathf.Lerp(playerHandLeftAnimator.GetFloat("speed"), speed, runAnimationLerp));
        playerHandRightAnimator.SetFloat("speed", Mathf.Lerp(playerHandRightAnimator.GetFloat("speed"), speed, runAnimationLerp));
    }

    private void AnimRun()
    {
        if (_runAnimationParam < 1f)
        {
            SetSpeed(0f);
        }
        else if (_runAnimationParam > 0.1f && _runAnimationParam <= runAnimationTreshold)
        {
            SetSpeed(0.5f);
        }
        else if (_runAnimationParam > runAnimationTreshold)
        {
            SetSpeed(1f);
        }
    }
}

