using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Transform itemHolder;
    public Item heldItem;

    public void HoldItem()
    {
        if (heldItem == null)
        {
            PlayerViewmodel.Instance.DisableViewmodel();
        }
        {
            PlayerViewmodel.Instance.EnableViewmodel();
        }
        ItemObject itemObject = Instantiate(heldItem.itemObject, itemHolder.position, itemHolder.rotation).GetComponent<ItemObject>();
        PlayerViewmodel.Instance.AdjustFingerTargets(itemObject.itemHoldPoints);
    }
}
