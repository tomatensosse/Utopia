using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryUI_Slot CurrentSlot { get; private set; }
    public ItemInstance itemInstance;
    public Image image;
    public TMP_Text amountText;
    public bool isNull = true;

    #region Drag and Drop

    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    #endregion

    public void Initialize(ItemInstance itemInstance, InventoryUI_Slot currentSlot)
    {
        isNull = false;

        this.itemInstance = itemInstance;
        image.sprite = itemInstance.itemReference.icon;

        if (itemInstance.itemReference.isStackable)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = itemInstance.amount.ToString();
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }

        CurrentSlot = currentSlot;
    }

    public void SetNewSlot(InventoryUI_Slot newSlot)
    {
        CurrentSlot = newSlot;
    }

    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }

    public void AppendAbility()
    {
        Debug.Log("Attatching Ability");
        Player.LocalPlayer.abilities.AddAbilitiesFromItem(itemInstance.itemReference);
    }

    public void DetatchAbility()
    {
        Debug.Log("Detatching Ability");
        Player.LocalPlayer.abilities.RemoveAbilitiesFromItem(itemInstance.itemReference);
    }
}