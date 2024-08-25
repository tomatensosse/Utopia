using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public Transform itemHolder;
    public Item heldItem;
    private PlayerViewmodel playerViewmodel;

    private void Awake()
    {
        playerViewmodel = PlayerViewmodel.Instance;
    }

    public void HoldItem()
    {
        if (heldItem == null)
        {
            playerViewmodel.DisableViewmodel();
        }
        {
            playerViewmodel.EnableViewmodel();
        }
        ItemObject itemObject = Instantiate(heldItem.itemObject, itemHolder.position, itemHolder.rotation).GetComponent<ItemObject>();
        playerViewmodel.AdjustFingerTargets(itemObject.itemHoldPoints);
        playerViewmodel.Wield();
    }
}
