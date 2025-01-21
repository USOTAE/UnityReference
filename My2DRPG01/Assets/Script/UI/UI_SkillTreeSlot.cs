using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler
{
    public bool unlocked;
    private UI ui;
    private Image skillIcon;

    [Header("Skill info")]
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;
    //[SerializeField] private TextMeshProUGUI skillType;
    [TextArea]
    [SerializeField] private string skillDescription;

    [Header("Unlock condition")]
    [SerializeField] private Color lockedSkillColor;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "UI_SkillTreeSlot - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    void Start()
    {
        ui = GetComponentInParent<UI>();
        skillIcon = GetComponent<Image>();

        skillIcon.color = lockedSkillColor;
        if (unlocked)
            skillIcon.color = Color.white;
    }

    public void UnlockSkillSlot()
    {
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log(shouldBeUnlocked[i].skillName + " is locked, "+ skillName + " can't unlock");
                return;
            }
        }

        for(int i = 0;i < shouldBeLocked.Length; i++)
        {
            if(shouldBeLocked[i].unlocked == true)
            {
                Debug.Log(shouldBeLocked[i].skillName + " is unlocked, "+ skillName + " can't unlock");
                return;
            }
        }

        if(PlayerManager.instance.player.HasEnoughCurrency(skillCost) == false)
        {
            Debug.Log("Have not enough currency, "+ skillName + " can't unlock");
            return;
        }

        unlocked = true;
        skillIcon.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillName, skillDescription, skillCost, skillIcon.sprite);
    }

    //todo save data

}
