using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Enemy", menuName = "Config/Enemy", order = 0)]
public class EnemySO : ScriptableObject
{
    [field: SerializeField] public GameObject Clone { get; private set; }
    [field: SerializeField] public ItemSO Weapon { get; private set; }
    [field: SerializeField] public float MaxHP { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Exp { get; private set; }
    [field: SerializeField] public int Reward { get; private set; }
    [field: SerializeField] public float AgentEnabledDistance { get; private set; }
    [field: SerializeField] public float ShootingDistance { get; private set; }
    [field: SerializeField] public float MeleeAttackDistance { get; private set; }

    [field: SerializeField] public DefenseType Defense;
}
