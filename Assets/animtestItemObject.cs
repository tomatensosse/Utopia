using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animtestItemObject : MonoBehaviour
{
    public Animator animtestItemAnimator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animtestItemAnimator.SetTrigger("ItemAnimation");
        }
    }
}
