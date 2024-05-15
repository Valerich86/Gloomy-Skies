using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Role", menuName = "Config/Role", order = 0)]

public class Role : ScriptableObject
{
    [field: SerializeField] public RoleType Type { get; private set; }
    [field: SerializeField] public GameObject Clone { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float MaxJumpForce { get; private set; }
    [field: SerializeField] public ItemSO Weapon { get; private set; }
    [field: SerializeField] public float MaxHP { get; private set; }
    [field: SerializeField] public int SuperStrikeDuration { get; private set; }
    [field: SerializeField] public float SuperStrikeOneHitDamage { get; private set; }
    [field: SerializeField] public int SuperStrikeRange { get; private set; }

    [Header("")]
    [Header("For 'Blacksmith' only:")]
    public ItemSO Shield;
}
