using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class StartScene : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private HudController _hud;
    [SerializeField] private InventoryController _inventory;
    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private Transform _teleportSpawnPosition;
    [SerializeField] private GameObject _teleportClone;
    [SerializeField] private GameObject _lightningsClone;

    private PlayerController _player;
    private GameManager _gameManager;
    void Start()
    {
        _gameManager = GameManager.Instance;
        StaticData.OnCameraChanged += ChangeCameraPriority;
        if (SaveService.IsLoading == true)
        {
            _player = Instantiate(_gameManager.CurrentRole.Clone, SaveService.StartPosition(), Quaternion.identity).GetComponent<PlayerController>();
            ActivateCamera();
            ChangeCameraPriority(10, 1);
            _hud.Construct(_player.GetComponent<HPController>(), _gameManager.CurrentRole.Icon);
            _inventory.Construct(_player);
            _hud.gameObject.SetActive(true);
            //_inventory.SetInventory();
            //_inventory.PutOn();
        } 
        else StartCoroutine(LevelStart());
    }

    private IEnumerator LevelStart()
    {
        SpawnPlayerCharacter();
        SpawnTeleport();
        SpawnLightnings();
        ActivateCamera();
        ChangeCameraPriority(1, 10);
        _hud.Construct(_player.GetComponent<HPController>(), _gameManager.CurrentRole.Icon);
        _inventory.Construct(_player);
        yield return new WaitForSeconds(7);
        _hud.gameObject.SetActive(true);
        _inventory.SetInventory();
        _inventory.PutOn();
        ChangeCameraPriority(10, 1);
    }

    void ActivateCamera()
    {
        _mainCamera.Follow = _player.transform;
        _mainCamera.LookAt = _player.transform;
        _startCamera.LookAt = _player.transform;
    }


    void ChangeCameraPriority(int mainCam_priority, int noiseCam_priority)
    {
        _mainCamera.Priority = mainCam_priority;
        _startCamera.Priority = noiseCam_priority;
    }

    void SpawnTeleport()
    {
        GameObject teleport = Instantiate(_teleportClone, _teleportSpawnPosition.position, Quaternion.identity);
        Destroy(teleport, 7);
    }

    void SpawnLightnings()
    {
        GameObject lightnings = Instantiate(_lightningsClone, _teleportSpawnPosition);
        Destroy(lightnings, 7);
    }
    void SpawnPlayerCharacter()
    {
        _player = Instantiate(_gameManager.CurrentRole.Clone, _playerSpawnPosition.position, _playerSpawnPosition.rotation).GetComponent<PlayerController>();
    }

}
