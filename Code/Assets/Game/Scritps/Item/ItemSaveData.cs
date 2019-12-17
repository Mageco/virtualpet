using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSaveData 
{
    public int id;
    public float value;
    public ItemSaveDataType itemType = ItemSaveDataType.None;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 scale = Vector3.one;
}

