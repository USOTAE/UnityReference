using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Skill_Dash : Skill
{
    [Header("Dash")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked {  get; private set; }

    //todo 技能扩展

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
    }

    protected override void CheckUnlock()
    {
        UnlockDash();
    }

    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;
        }
    }

}
