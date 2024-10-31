using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool isUsable;
}

[CreateAssetMenu(fileName = "NewInformation", menuName = "Inventory/Information")]
public class InformationData : ScriptableObject
{
    public string infoName;
    public string description;
}


