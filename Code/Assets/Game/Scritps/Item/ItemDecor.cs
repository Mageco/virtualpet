using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDecor : BaseFloorItem
{
    protected override void OnClick()
    {
        base.OnClick();
        animator.Play("Active", 0);
    }
}
