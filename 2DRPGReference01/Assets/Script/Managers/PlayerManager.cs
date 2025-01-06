using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public bool HaveEnoughMoney(int _price)
    {
        if(_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency -= _price;
        return true;
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}


//public static PlayerManager instance;
//- `instance` 是一个静态变量，意味着它属于类而不是类的实例，可以在整个应用程序中全局访问。
//- 这种模式被称为单例模式（Singleton），用于确保类只有一个实例，并提供一个全局访问点。
//- `Awake` 是 Unity 中的一个生命周期方法，当脚本实例被加载时调用。
//- 该方法的作用是确保 `PlayerManager` 的单例模式。
//- 如果 `instance` 已经存在（即已经有了 `PlayerManager` 的实例），则通过 `Destroy(instance.gameObject)` 销毁之前的实例。这通常是在场景切换或重载时使用，以避免多个实例并防止状态混淆。
//- 如果 `instance` 为空，则将当前实例 (`this`) 赋值给 `instance`，建立单例。
