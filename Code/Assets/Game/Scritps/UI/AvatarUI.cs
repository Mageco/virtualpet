using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    public Image avatar;
    public void LoadAvatar(Sprite s)
    {
        avatar.sprite = s;
    }
}
