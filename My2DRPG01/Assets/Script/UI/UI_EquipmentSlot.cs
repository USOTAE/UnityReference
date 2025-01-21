using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;
        
        if (item.data.itemType == ItemType.Equipment)
        {
            InventoryManager.instance.EquipItem(item.data);
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.equipmentToolTip.ShowToolTip(item.data);
    }

    //todo unequip item
}
