using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;

    //[SerializeField] private int defaultFontSize = 32;

    public void ShowToolTip(ItemData _item)
    {
        if (_item == null || itemName.text == _item.itemName)
            return;

        itemName.text = _item.itemName;
        itemType.text = _item.itemType.ToString();
        itemDescription.text = _item.GetDescription();
        itemIcon.sprite = _item.itemIcon;

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
