using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInformation", menuName = "Inventory/InformationData")]
public class InformationData : ScriptableObject
{
    public string infoName;
    public string combatInfoName;
    public string description;
}
