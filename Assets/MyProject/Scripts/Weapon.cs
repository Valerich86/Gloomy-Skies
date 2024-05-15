using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Config/Weapon", order = 0)]

public class Weapon : ScriptableObject
{
    [field: SerializeField] public AttackType Type { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public float MeleeRange { get; private set; }

    [Header("")]
    [Header("For 'Bow' only:")]
    public GameObject PlayerArrowClone;
    public GameObject EnemyArrowClone;
    public float BowShotForce;
    public int StartArrowAmount;

    [Header("")]
    [Header("For 'CombatAxe' only:")]
    public GameObject AxeClone;
    public float ForceZ = 200;
    public float ForceY = 50;
}
