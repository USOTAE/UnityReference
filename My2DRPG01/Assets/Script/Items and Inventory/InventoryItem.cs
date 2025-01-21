using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public int stackSize;   //�ֿ�һ����Ʒ�Ķѵ�����
    public ItemData data;

    public InventoryItem(ItemData _newData)
    {
        this.data = _newData;
        AddStack();
    }

    public void AddStack(int _amount=1)
    {
        stackSize += _amount;
    }

    public void RemoveStack(int _amount=1)
    {
        stackSize -= _amount;
    }
}
