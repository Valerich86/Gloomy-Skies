
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IMortal
{
    [field: SerializeField] public EnemySO Enemy { get; private set; }
    [field: SerializeField] public GameObject Weapon { get; private set; }

    private AttackController _attackController;
    private HPController _hpController;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _isSearching = true;
    private float _maxSpeed;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>().transform;
        StaticData.OnPlayerDying += OnPlayerDying;
        _agent = GetComponent<NavMeshAgent>();
        _attackController = GetComponent<AttackController>();
        _hpController = GetComponent<HPController>();   
        _animator = GetComponent<Animator>();
        _attackController.SetEnemyWeapon(Enemy.Weapon);
        _hpController.SetStartHealth(Enemy.MaxHP, Enemy.MaxHP);
        _maxSpeed = Enemy.Speed;
        FindNewPoint();
    }

    private void OnPlayerDying()
    {
        _agent.enabled = false;
        _animator.SetFloat("Speed", 0);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        if (_agent.enabled)
        {
            if (_isSearching && !_agent.pathPending && _agent.remainingDistance <= 3) FindNewPoint();
            else if (distance <= Enemy.AgentEnabledDistance)
            {
                _isSearching = false;
                if (Enemy.Weapon.WeaponSO.Type == AttackType.long_range)
                {
                    if (distance <= Enemy.ShootingDistance && distance > _agent.stoppingDistance)
                    {
                        _agent.SetDestination(_player.position);
                        transform.LookAt(_player.position);
                        _animator.SetFloat("Speed", 1);
                        _agent.speed = _maxSpeed;
                        _attackController.Attack();
                    }
                    else if (distance <= Enemy.MeleeAttackDistance)
                    {
                        _agent.SetDestination(_player.position - _player.forward * 20f);
                        _animator.SetFloat("Speed", 1);
                        _agent.speed = _maxSpeed;
                    }
                    else
                    {
                        _agent.SetDestination(_player.position);
                        transform.LookAt(_player.position);
                        _animator.SetFloat("Speed", 0);
                        _agent.speed = 0;
                        _attackController.Attack();
                    }
                }
                else
                {
                    _agent.SetDestination(_player.position);
                    transform.LookAt(_player.position);
                    if (distance <= Enemy.MeleeAttackDistance + 2f && distance >= Enemy.MeleeAttackDistance + 1.8f)
                    {
                        _animator.SetTrigger("RollForward");
                        _agent.speed = _maxSpeed;
                    }
                    else if (distance <= Enemy.MeleeAttackDistance)
                    {
                        _animator.SetFloat("Speed", 0.1f);
                        _agent.speed = _maxSpeed / 3;
                        _attackController.Attack();
                    }
                    else
                    {
                        _animator.SetFloat("Speed", 1);
                        _agent.speed = _maxSpeed;
                    }
                }
            }
            else if (distance > Enemy.AgentEnabledDistance) _isSearching = true;
        }
    }

    private void FindNewPoint()
    {
        NavMeshTriangulation data = NavMesh.CalculateTriangulation();
        int index = Random.Range(0, data.vertices.Length);
        Vector3 randomPoint = data.vertices[index];
        _animator.SetFloat("Speed", 0.1f);
        _agent.speed = _maxSpeed / 3;
        _agent.SetDestination(randomPoint);
    }


    public void Dying()
    {
        StaticData.OnEnemyDying?.Invoke(this);
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 30f);
    }

}
