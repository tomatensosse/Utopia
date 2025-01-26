using System.Collections;
using UnityEngine;

public class RegisterTransformToWorld : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndAddSelf());
    }

    IEnumerator WaitAndAddSelf()
    {
        yield return new WaitUntil(() => World.Ready && MeshGenerator.Ready);
        World.RegisterTransform(this.transform);
    }
}