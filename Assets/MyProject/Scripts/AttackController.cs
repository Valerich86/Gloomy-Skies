
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public enum AttackType { melee, long_range, melee_long }
public enum ArrowType { PlayerArrow, EnemyArrow }
public class AttackController : MonoBehaviour
{
    [HideInInspector] public bool IsAttacking { get; private set; }
    [HideInInspector] public int ArrowAmount { get; private set; }

    [SerializeField] private LayerMask _attackingMask;
    [SerializeField] private Transform _arrowPoint;
    [SerializeField] private Transform _axePoint;
    [SerializeField] private Transform _superStrikePoint;
    [SerializeField] private GameObject _strikeVFX;
    
    private Collider[] _hits = new Collider[10];
    private Animator _animator;
    private Weapon _weapon;
    private GameObject _currentWeapon;
    private GameObject _axe;
    private Transform _rightHand;
    private Transform _leftHand;
    private Transform _head;
    private Transform _spine;
    private Vector3 _rangeOffset = new Vector3(0, 1, 0);
    private int _arrowAmount;
    private int _ssRange;
    private int _ssDuration;
    private float _ssDamage;
    private bool _ss_isActive = false;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _rightHand = FindObjectOfType<WeaponHand>().transform;
        _leftHand = FindObjectOfType<ShieldHand>().transform;
        _head = FindObjectOfType<Head>().transform;
        _spine = FindObjectOfType<Spine>().transform;
        _animator = gameObject.GetComponent<Animator>();
        _ssDuration = _gameManager.CurrentRole.SuperStrikeDuration;
        _ssDamage = _gameManager.CurrentRole.SuperStrikeOneHitDamage;
        _ssRange = _gameManager.CurrentRole.SuperStrikeRange;
        ResetAttack();
    }

    private void Update()
    {
        if (_ss_isActive)
        {
            //transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * 10f, 2f * Time.deltaTime);
            transform.Rotate(0, 1, 0);
        }
    }
    public void ResetAttack() => IsAttacking = false;


    public void SetPlayerWeapon(ItemSO weapon)
    {
        if (_currentWeapon != null) Destroy(_currentWeapon);
        if (weapon.WeaponSO.Type == AttackType.long_range) _currentWeapon = Instantiate(weapon.Clone, _leftHand);
        else if (weapon.WeaponSO.Type == AttackType.melee || weapon.WeaponSO.Type == AttackType.melee_long) _currentWeapon = Instantiate(weapon.Clone, _rightHand);
        if (_currentWeapon.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (_currentWeapon.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        _weapon = weapon.WeaponSO;
        ResetAttack();
    }

    public void SetPlayerShield(ItemSO shield)
    {
        GameObject s = null;
        if (_currentWeapon == null || _weapon.Type != AttackType.long_range) s = Instantiate(shield.Clone, _leftHand);
        else s = Instantiate(shield.Clone, _rightHand);
        if (s.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (s.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        if (TryGetComponent<PlayerController>(out PlayerController player)) player.ChangeDefenceType(DefenseType.Shield);
    }

    public void SetPlayerQuiver(ItemSO quiver)
    {
        GameObject q = null;
        q = Instantiate(quiver.Clone, _spine);
        if (q.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (q.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
    }

    public void SetPlayerHelmet(ItemSO helmet)
    {
        GameObject h = null;
        h = Instantiate(helmet.Clone, _head);
        if (h.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (h.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        if (TryGetComponent<HPController>(out HPController hp)) hp.SetHelmet();
    }
    public void Attack()
    {
        if (IsAttacking == false)
        {
            if (_weapon == null) return;
            else
            {
                if (_weapon.Type == AttackType.melee || _weapon.Type == AttackType.melee_long)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("Combo");
                }
                if (_weapon.Type == AttackType.long_range && _arrowAmount > 0)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("BowShot");
                    Invoke("ArrowSpawn", .5f);
                }
                Invoke("ResetAttack", _weapon.Cooldown);
            }
        }
    }


    public void AlternativeAttack()
    {
        if (IsAttacking == false)
        {
            if (_weapon == null) return;
            else
            {
                if (_weapon.Type == AttackType.melee_long)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("AxeThrow");
                    Invoke("CheckWeaponMesh", 5f);
                }
            }
        }
    }

    public void SuperStrike() => StartCoroutine(SuperStrikeAction());

    private IEnumerator SuperStrikeAction()
    {
        GetComponent<PlayerController>().ResetMovement(false);
        _animator.SetTrigger("SuperStrike");
        StaticData.OnCameraChanged?.Invoke(1, 10);
        yield return new WaitForSeconds(3);
        GetComponent<CharacterController>().radius = 3f;
        _animator.SetBool("Levitation", true);
        GetComponent<HPController>().SetInvulnerability(true);
        _ss_isActive = true;
        GameObject vfx = Instantiate(_strikeVFX, _superStrikePoint.position + Vector3.up, _superStrikePoint.rotation);
        for (int i = 0; i < _ssDuration; i++)
        {
            yield return new WaitForSeconds(1);
            CheckSuperStrikeDamage(_ssRange);
        }
        yield return new WaitForSeconds(2);
        _ss_isActive = false;
        GetComponent<CharacterController>().radius = 0.3f;
        GetComponent<PlayerController>().ResetMovement(true);
        GetComponent<HPController>().SetInvulnerability(false);
        yield return new WaitForSeconds(1);
        _animator.SetBool("Levitation", false);
        StaticData.OnSuperStrikeApplied?.Invoke();
        StaticData.OnCameraChanged?.Invoke(10, 0);
        Destroy(vfx, 10f);
    }


    private void CheckSuperStrikeDamage(int range)
    {
        int count = Physics.OverlapSphereNonAlloc(_superStrikePoint.position, range, _hits, _attackingMask);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<HPController>(out HPController hpController))
            {
                hpController.GetComponent<Rigidbody>().isKinematic = true;
                hpController.GetComponent<Rigidbody>().AddExplosionForce(500, transform.position, _ssRange);
                hpController.TakeDamage(_ssDamage);
            }
        }
    }
    private void CheckWeaponMesh()
    {
        if (_currentWeapon.GetComponent<MeshRenderer>().enabled == false)
            OnAxeCatch();
    }


    public void OnAxeThrow()
    {
        _currentWeapon.GetComponent<MeshRenderer>().enabled = false;
        _axe = Instantiate(_weapon.AxeClone, _axePoint.position, _axePoint.rotation);
        Rigidbody rb = _axe.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * _weapon.ForceZ + transform.up * _weapon.ForceY, ForceMode.Impulse);
    }

    public void OnAxeCatch()
    {
        Destroy(_axe);
        _currentWeapon.GetComponent<MeshRenderer>().enabled = true;
        ResetAttack();
    }

    void MeleeAttackCheck()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + _rangeOffset, _weapon.MeleeRange, _hits, _attackingMask);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<HPController>(out HPController hpController))
            {
                hpController.TakeDamage(_weapon.Damage);
            }
        }
    }

    void ArrowSpawn()
    {
        GameObject arrow = null;
        if (TryGetComponent<PlayerController>(out PlayerController p))
        {
            arrow = Instantiate(_weapon.PlayerArrowClone, _arrowPoint.position, _arrowPoint.rotation);
            StaticData.OnArrowAmountChanged?.Invoke(-1);
        }
        if (TryGetComponent<EnemyController>(out EnemyController e))
            arrow = Instantiate(_weapon.EnemyArrowClone, _arrowPoint.position, _arrowPoint.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * _weapon.BowShotForce, ForceMode.Impulse);
    }

    public void SetCurrentArrowsAmount(int amount) => _arrowAmount = amount;
    internal void SetEnemyWeapon(ItemSO weapon)
    {
        _weapon = weapon.WeaponSO;
        if (_weapon.Type == AttackType.long_range) SetCurrentArrowsAmount(100);
    }



    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.forward + _weapon.WeaponRange, _weapon.Range);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position + transform.forward + _weapon.WeaponRange, 0.2f);
    //}
}
