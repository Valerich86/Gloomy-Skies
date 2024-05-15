using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Config/Item", order = 0)]

public class ItemSO : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public string Tytle { get; private set; }
    [field: SerializeField] public ItemAssignment Assignment { get; private set; }
    [field: SerializeField] public ItemType Type { get; private set; }
    [field: SerializeField] public GameObject Clone { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Price { get; private set; }

    [Header("")]
    [Header("For 'Cure' only:")]
     public int HealPercents;

    [Header("")]
    [Header("For 'Weapon' only:")]
    public Weapon WeaponSO;

}
