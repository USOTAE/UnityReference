using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDropCount; //可能掉落的道具的数量
    [SerializeField] private ItemData[] possibleItemDropList; //可能掉落的道具的列表,掉落的物品在该列表内,需要给怪物配置
    [SerializeField] private GameObject dropItemPrefab;
    private List<ItemData> dropList = new List<ItemData>(); //实际可掉落的物品的列表

    public virtual void GenerateDropList()
    {
        for (int i = 0;i<possibleItemDropList.Length;i++)
        {
            if (Random.Range(0, 100) <= possibleItemDropList[i].dropChance)
            {
                dropList.Add(possibleItemDropList[i]);
            }
        }

        for(int i = 0; i<possibleItemDropCount; i++)
        {
            if (dropList.Count > 0)
            {
                ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
                dropList.Remove(randomItem);
                DropItem(randomItem);
            }
            else
            {
                Debug.Log("No item can drop");
            }
        }
    }

    public virtual void DropItem(ItemData _item)
    {
        GameObject newDrop = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));
        newDrop.GetComponent<ItemObject>().SetupItem(_item, randomVelocity);
    }
}