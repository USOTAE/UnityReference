using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_PowerUp : Skill
{
    [Header("Power up")]
    [SerializeField] private UI_SkillTreeSlot powerUpUnlockButton;
    public bool powerUpUnlocked {  get; private set; }

    protected override void Start()
    {
        base.Start();

        powerUpUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPowerUp);
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void CheckUnlock()
    {
        UnlockPowerUp();
    }

    private void UnlockPowerUp()
    {
        if (powerUpUnlockButton.unlocked)
            powerUpUnlocked = true;
    }
}
