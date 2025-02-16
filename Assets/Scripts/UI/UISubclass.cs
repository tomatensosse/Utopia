using System.Collections.Generic;
using UnityEngine;

public abstract class UISubclass : MonoBehaviour
{
    public List<GameObject> elements;
    public KeyCode toggleKey;

    public virtual void EnableUI()
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(true);
        }
    }

    public virtual void DisableUI()
    {
        foreach (GameObject element in elements)
        {
            element.SetActive(false);
        }
    }
}