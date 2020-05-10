using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeItem : BaseFloorItem
{
      List<FruitItem> fruits = new List<FruitItem>();

    public override void Load(PlayerItem item)
    {
        base.Load(item);
        FruitItem[] temp = this.transform.GetComponentsInChildren<FruitItem>(true);
        for(int i = 0; i < temp.Length; i++)
        {
            fruits.Add(temp[i]);
            temp[i].id = item.realId * 100 + i;
            temp[i].timeGrow = DataHolder.GetItem(item.itemId).value;
        }
    }
}
