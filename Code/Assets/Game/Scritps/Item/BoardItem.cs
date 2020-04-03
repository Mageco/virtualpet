﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardItem : BaseFloorItem
{
    public TextMeshPro playerName;
    protected override void OnClick()
    {
        base.OnClick();
        UIManager.instance.OnHouseNamePanel();
        animator.Play("Active", 0);
    }

    protected override void Update()
    {
        base.Update();
        playerName.text = GameManager.instance.myPlayer.playerName;
    }
}
