using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MainObject : MonoBehaviour
{
    public Animator animator;

    public AnimationClip itemAction;
    public int rightLayerIndex = 1;

    private AnimatorOverrideController overrideController;

    void Start()
    {
        AnimatorController baseController = animator.runtimeAnimatorController as AnimatorController;

        AnimationClip originalClip = null;
        if (baseController != null)
        {
            for (int layerIndex = 0; layerIndex < baseController.layers.Length; layerIndex++)
            {
                var states = baseController.layers[layerIndex].stateMachine.states;
                foreach (var stateInfo in states)
                {
                    if (stateInfo.state.name == "ItemAction")
                    {
                        stateInfo.state.motion = itemAction;
                        Debug.Log("Hell yeah");

                        EditorUtility.SetDirty(baseController);
                        break;
                    }
                }
                if (originalClip != null)
                {
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayItemAnimation();
        }
    }

    private void PlayItemAnimation()
    {
        animator.SetTrigger("ItemAction");
    }
}
