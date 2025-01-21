using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;   //角色穿戴的装备   
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> equipmentBag;    //角色的装备背包 对应ui中character ui内的equipment bag
    public Dictionary<ItemData, InventoryItem> equipmentBagDictionary;

    public List<InventoryItem> stash;   //物品的背包 对应ui中inventory页签下的stash
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform equipmentBagSlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform statsSlotParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject equipmentSlotPrefab;

    //todo slot ui
    private UI_EquipmentSlot[] equipmentSlots;
    private List<UI_ItemSlot> equipmentBagSlots = new List<UI_ItemSlot>();
    private List<UI_ItemSlot> stashItemSlots = new List<UI_ItemSlot>();
    private UI_StatsSlot[] statsSlots;

    //todo use cooldown

    [Header("Data base")]
    public List<ItemData> itemDatabase;
    public List<InventoryItem> loadedItems;
    //todo equipment data base

    private const int maxStackSize = 9999;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        equipmentBag = new List<InventoryItem>();
        equipmentBagDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipmentSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        equipmentBagSlots = new List<UI_ItemSlot>(equipmentBagSlotParent.GetComponentsInChildren<UI_ItemSlot>());
        stashItemSlots = new List<UI_ItemSlot>(stashSlotParent.GetComponentsInChildren<UI_ItemSlot>());
        statsSlots = statsSlotParent.GetComponentsInChildren<UI_StatsSlot>();
    }

    //调试用
    //private void Update()
    //{
    //    //打印出当前stash内有多少道具
    //    foreach (var item in stash)
    //    {
    //        Debug.Log(item.data.name + " " + item.stackSize);
    //    }
    //}

    public void AddItem(ItemData _item, int _amount = 1)
    {
        if (_item.itemType == ItemType.Equipment)
        {
            AddToEquipmentBag(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item, _amount);
        }

        UpdateUISlot();
    }

    public void RemoveItem(ItemData _item, int _amount = 1)
    {
        if (equipmentBagDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize >= _amount)
            {
                if (value.stackSize == _amount)
                {
                    equipmentBag.Remove(value);
                    equipmentBagDictionary.Remove(_item);
                }
                else
                {
                    value.RemoveStack(_amount);
                }
            }
            else
            {
                Debug.Log("物品不足移除");
            }
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize >= _amount)
            {
                if (stashValue.stackSize == _amount)
                {
                    stash.Remove(stashValue);
                    stashDictionary.Remove(_item);
                }
                else
                {
                    stashValue.RemoveStack(_amount);
                }
            }
            else
            {
                Debug.Log("物品不足移除");
            }
        }

        UpdateUISlot();
    }

    public bool CanAddItem()
    {
        //if (equipmentBag.Count >= equipmentBagSlots.Length)   //bug：背包满时即使时背包内已经有的key也不能捡
        //    return false;
        return true;
    }

    private void AddToEquipmentBag(ItemData _item)
    {
        if (equipmentBagDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize < maxStackSize)
                value.AddStack();
            else
                Debug.Log(_item.itemName + "are maxed");
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            equipmentBag.Add(newItem);
            equipmentBagDictionary.Add(_item, newItem);

            if (equipmentBag.Count > equipmentBagSlotParent.GetComponentsInChildren<UI_ItemSlot>().Length)
                AddNewItemSlot(_item);
        }
    }

    private void AddToStash(ItemData _item, int _amount = 1)
    {
        if (_amount > maxStackSize)
            return;

        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            //if (value.stackSize < maxStackSize)
            //    value.AddStack();
            //else
            //    Debug.Log(_item.itemName + "are maxed");
            if (value.stackSize + _amount > maxStackSize)
                return;
            value.AddStack(_amount);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
            if (_amount > 1)
                value.AddStack(_amount - 1);

            AddNewItemSlot(_item);
        }
    }

    private void AddNewItemSlot(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
        {
            GameObject newEquipmentSlotObject = Instantiate(equipmentSlotPrefab, equipmentBagSlotParent);
            UI_EquipmentSlot newEquipmentSlot = newEquipmentSlotObject.GetComponent<UI_EquipmentSlot>();
            equipmentBagSlots.Add(newEquipmentSlot);
        }
        else if (_item.itemType == ItemType.Material)
        {
            GameObject newItemSlotObject = Instantiate(itemSlotPrefab, stashSlotParent);
            UI_ItemSlot newItemSlot = newItemSlotObject.GetComponent<UI_ItemSlot>();
            stashItemSlots.Add(newItemSlot);
        }
        else
        {
            Debug.LogError("Add new Item slot error: unknow item type");
        }
    }

    private void UpdateUISlot()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlots[i].slotType)
                {
                    equipmentSlots[i].UpdateSlot(item.Value);
                }
            }
        }

        //先清空所有item slot信息
        for (int i = 0; i < equipmentBagSlots.Count; i++)
        {
            equipmentBagSlots[i].CleanUpSlot();
        }
        //再按照库存进行更新
        for (int i = 0; i < equipmentBag.Count; i++)
        {
            equipmentBagSlots[i].UpdateSlot(equipmentBag[i]);
        }

        for (int i = 0; i < stashItemSlots.Count; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statsSlots.Length; i++)
        {
            statsSlots[i].UpdateStatsValueUI();
        }
    }

    public virtual void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        ItemData_Equipment oldEquipment = null;

        InventoryItem newItem = new InventoryItem(newEquipment);

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
                break;
            }
        }

        if (oldEquipment != null)
        {
            UnEquipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItem(_item);

        UpdateUISlot();
    }

    public virtual void UnEquipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);

            _itemToRemove.RemoveModifiers();
        }
    }


    public virtual int GetStashItemAmount(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            return value.stackSize;
        }

        return 0;
    }

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipmentItem = null;

        foreach (KeyValuePair<ItemData_Equipment,InventoryItem> item in equipmentDictionary)
        {
            if(item.Key.equipmentType == _type)
            {
                equipmentItem = item.Key;
            }
        }

        return equipmentItem;
    }
}
