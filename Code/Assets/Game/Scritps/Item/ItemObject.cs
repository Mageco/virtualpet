using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
	public ItemInteractType interactType = ItemInteractType.None;
}

public enum ItemInteractType {None,Drag,Drop};
