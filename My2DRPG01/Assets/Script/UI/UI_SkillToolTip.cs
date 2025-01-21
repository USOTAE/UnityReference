using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCost;
    //[SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private Image skillIcon;

    public void ShowToolTip(string _skillName, string _skillDescription, int _skillCost, Sprite _skillIcon)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;
        skillCost.text = "Cost: " + _skillCost;
        skillIcon.sprite = _skillIcon;

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
