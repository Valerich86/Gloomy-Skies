using Cinemachine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public interface IMortal
{
    public void Dying();
}

public enum DefenseType { Default, Shield}

public class PlayerController : MonoBehaviour, IMortal
{
    //public event Action OnPlayerDying;

    private GameManager _gameManager;
    private CharacterController _controller;
    private HPController _hpController;
    private Animator _animator;
    private AttackController _attackController;
    private DefenseType _defenceType;
    private float _jumpForce = 0;
    private bool _isDead = false;
    private bool _canMove = false;
    private bool _canApplySS = false;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _attackController = GetComponent<AttackController>();
        _hpController = GetComponent<HPController>();
        _hpController.SetStartHealth(_gameManager.CurrentHP, _gameManager.MaxHP);
        _defenceType = DefenseType.Default;
        if (SaveService.IsLoading == false) StartCoroutine(OnStart());
        else _canMove = true;
        Time.timeScale = 1;
    }

    private IEnumerator OnStart()
    {
        _animator.Play("Die");
        yield return new WaitForSeconds(9);
        _animator.Play("Movement");
        _canMove = true;
    }

    void Update()
    {
        if (_canMove)
        {
            Movement();
            Attack();
            Defense();
            Jump();
        }
    }

    public void ApplySS(bool value) => _canApplySS = value;

    public void ChangeDefenceType(DefenseType type) => _defenceType = type;

    private void Defense()
    {
        if (Input.GetKeyDown(KeyCode.E) && _defenceType == DefenseType.Shield)
        {
            _hpController.SetBlock();
        }
        else if (Input.GetKeyUp(KeyCode.E) && _defenceType == DefenseType.Shield)
        {
            _hpController.ResetBlock();
        }
        if ((Input.GetKey(KeyCode.W)) && Input.GetKeyDown(KeyCode.E) && _defenceType == DefenseType.Default)
        {
            _animator.SetTrigger("RollForward");
        }
        if ((Input.GetKey(KeyCode.S)) && Input.GetKeyDown(KeyCode.Q))
        {
            _animator.SetTrigger("RollBack");
        }
    }

    void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = transform.TransformDirection(0, _jumpForce, vertical);
        if (!_isDead)
        {
            if (Input.GetKey(KeyCode.A)) transform.Rotate(Vector3.up, -_gameManager.CurrentRole.RotationSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D)) transform.Rotate(Vector3.up, _gameManager.CurrentRole.RotationSpeed * Time.deltaTime);

            _controller.Move(direction * _gameManager.CurrentRole.Speed * Time.deltaTime);
            _animator.SetFloat("Speed", vertical);
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !_isDead) _attackController.Attack();
        if (Input.GetMouseButtonDown(1) && !_isDead) _attackController.AlternativeAttack();
        if (Input.GetKeyDown(KeyCode.Q) && !_isDead && _canApplySS)
        {
            ApplySS(false);
            _attackController.SuperStrike();
        } 
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpForce = _gameManager.CurrentRole.MaxJumpForce;
            _animator.SetTrigger("Jump");
            Invoke("ReturnDirectionY", 1f);
        }
    }

    private void ReturnDirectionY() => _jumpForce = 0;
    

    public void Dying()
    {
        _isDead = true;
        StaticData.OnPlayerDying?.Invoke();
    }


    public void ResetMovement(bool value)
    {
        _canMove = value;
        _animator.SetFloat("Speed", 0);
    }
}
