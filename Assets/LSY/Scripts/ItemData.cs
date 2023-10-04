using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable,
    Installable
}

public enum ConsumableType
{
    Hunger,
    Thirst,
    Health,
    Stamina
    // Immune, Resistance (Fire, Cold, Poison) 
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType Type;
    public int Value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string DisplayName;
    public string Description;
    public ItemType Type;
    public Sprite Icon;
    public GameObject DropPrefab;
    public GameObject WeaponPrefab;
    public GameObject InstallablePrefab;

    [Header("Stacking")]
    public bool IsStackable;
    public int MaxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] Consumables;
}
