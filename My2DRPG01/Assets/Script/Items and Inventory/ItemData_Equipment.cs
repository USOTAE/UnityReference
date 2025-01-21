using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemEffectCooldown;
    public ItemEffects[] itemEffects;

    [Header("Offensive stats")]
    public int attack;
    public int critChance;
    public int critPower;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("Survival stats")]
    public int hp;
    public int pdef;
    public int mdef;

    //todo craft
    //[Header("Craft requirements")]
    //public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.attack.AddModifier(attack);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);
        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
        playerStats.maxHp.AddModifier(hp);
        playerStats.pdef.AddModifier(pdef);
        playerStats.mdef.AddModifier(mdef);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.attack.RemoveModifier(attack);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);
        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
        playerStats.maxHp.RemoveModifier(hp);
        playerStats.pdef.RemoveModifier(pdef);
        playerStats.mdef.RemoveModifier(mdef);
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDesctiption(attack, "Attack");
        AddItemDesctiption(critChance, "Crit.Chance");
        AddItemDesctiption(critPower, "Crit.Power");
        AddItemDesctiption(fireDamage, "Value.Fire");
        AddItemDesctiption(iceDamage, "Value.Ice");
        AddItemDesctiption(lightningDamage, "Value.Lightning");
        AddItemDesctiption(hp, "Hp");
        AddItemDesctiption(pdef, "Physical Defense");
        AddItemDesctiption(mdef, "Magical Defense");

        sb.AppendLine();
        sb.AppendLine("Item Description " + itemDescription);
        descriptionLength++;

        //²»×ã5ÐÐÌî³ä¿Õ¸ñ
        //if (descriptionLength < 5)
        //{
        //    for (int i = 0; i < 5 - descriptionLength; i++)
        //    {
        //        sb.AppendLine();
        //        sb.Append("");
        //    }
        //}

        return sb.ToString();
    }

    private void AddItemDesctiption(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);
            else
                sb.Append("- " + _value + " " + _name);

            descriptionLength++;
        }
    }

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

}
