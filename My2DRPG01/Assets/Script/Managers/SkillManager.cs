using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public Skill_Sword sword {  get; private set; }
    public Skill_PowerUp powerUp { get; private set; }
    public Skill_Clone clone { get; private set; }
    public Skill_Dash dash { get; private set; }
    public Skill_Lightsaber lightsaber { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }


    private void Start()
    {
        sword = GetComponent<Skill_Sword>();
        powerUp = GetComponent<Skill_PowerUp>();
        clone = GetComponent<Skill_Clone>();
        dash = GetComponent<Skill_Dash>();
        lightsaber = GetComponent<Skill_Lightsaber>();
    }
}
