using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStatus : MonoBehaviour
{
    public SpriteRenderer progress;
    public SpriteRenderer icon;

    public void Load(IconStatus status)
    {
        icon.sprite = Resources.Load<Sprite>("Icons/ItemType/" + status.ToString()) as Sprite;
    }
    public void SetProgress(float v)
    {
        progress.transform.localScale = new Vector3(1,v, 1);
        if (v < 0.1)
            progress.color = Color.red;
        else if (v < 0.3)
            progress.color = Color.yellow;
        else
            progress.color = Color.green;
    }
}
