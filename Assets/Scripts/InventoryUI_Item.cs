using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI_Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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

    public void Initialize(ItemInstance itemInstance)
    {
        isNull = false;

        this.itemInstance = itemInstance;
        image.sprite = itemInstance.itemReference.icon;
        amountText.text = itemInstance.amount.ToString();
    }

    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }
}