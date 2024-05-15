using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class ArmoryController : MonoBehaviour
{
    [SerializeField] private GameObject _blacksmith;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private CinemachineVirtualCamera _camera1;
    [SerializeField] private CinemachineVirtualCamera _camera2;

    private NavMeshAgent _agent;
    private Animator _animator;
    private InventoryController _inventory;
    private bool _isOnPosition = true;
    private bool _isPlayerIn = false;
    public void Start()
    {
        _animator = _blacksmith.GetComponent<Animator>();
        _agent = _blacksmith.GetComponent<NavMeshAgent>();
        _animator.SetFloat("Speed", 0f);
    }


    private void Update()
    {
        if (_isPlayerIn && Input.GetKeyDown(KeyCode.F)) ArmoryOut();

        if (!_isOnPosition && _agent.remainingDistance <= 0.5)
        {
            _isOnPosition = true;
            _animator.SetFloat("Speed", 0f);
            if (_isPlayerIn)
            {
                Transform player = FindObjectOfType<PlayerController>().transform;
                _blacksmith.transform.LookAt(player);
                StartCoroutine(Dialog());
            }
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            _isPlayerIn = true;
            player.ResetMovement(false);
            StaticData.OnCameraChanged?.Invoke(1, 0);
            ChangePriority(8, 1);
            _animator.SetFloat("Speed", 0.1f);
            _agent.SetDestination(transform.position);
            _isOnPosition = false;
            _inventory = FindObjectOfType<InventoryController>();
            _inventory.ChangeStatus(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            _isPlayerIn = false;
            _animator.SetFloat("Speed", 0.1f);
            _agent.SetDestination(_startPoint.position);
            _isOnPosition = false;
        }
    }

    IEnumerator Dialog()
    {
        StaticData.OnGlobalHintChanged?.Invoke("Продаю то, что выковал сам и покупаю всё остальное", 3);
        yield return new WaitForSeconds(3);
        StaticData.OnGlobalHintChanged?.Invoke("' ЛКМ ' по товару - посмотреть информацию \n' ЛКМ ' в инвентаре - продать 1 лут\n' R ' - продать весь лут\n' F ' - выход", 300);
        ChangePriority(1, 8);
    }

    private void ArmoryOut()
    {
        _inventory.ChangeStatus(true);
        FindObjectOfType<PlayerController>().ResetMovement(true);
        StaticData.OnGlobalHintChanged?.Invoke(string.Empty, 0);
        StaticData.OnCameraChanged?.Invoke(10, 0);
    }
    private void ChangePriority(int cam1, int cam2)
    {
        _camera1.Priority = cam1;
        _camera2.Priority = cam2;
    }
}
