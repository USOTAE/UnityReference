using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected Image itemBackgroundImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;
        itemBackgroundImage.color = Color.white;

        if(item!=null)
        {
            itemImage.sprite = item.data.itemIcon;
            itemBackgroundImage.sprite = item.data.itemBackgroundIcon;
            if (item.stackSize > 1)
                itemText.text = item.stackSize.ToString();
            else
                itemText.text = "";
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemBackgroundImage.sprite = null;
        itemBackgroundImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            InventoryManager.instance.RemoveItem(item.data);
            return;
        }

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) 
            return;

        ui.itemToolTip.ShowToolTip(item.data);
    }
}
