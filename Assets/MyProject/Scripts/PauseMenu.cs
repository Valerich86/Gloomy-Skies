using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    private Button[] _buttons;
    private Canvas _pauseMenu;

    private void OnEnable()
    {
        _pauseMenu = GetComponent<Canvas>();
        _pauseMenu.enabled = false;
        _buttons = GetComponentsInChildren<Button>();
        _buttons[0].onClick.AddListener(OnContinueClicked);
        _buttons[1].onClick.AddListener(OnSaveClicked);
        _buttons[2].onClick.AddListener(OnMainMenuClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.enabled = true;
            Time.timeScale = 0;
        } 
    }

    private void OnContinueClicked()
    {
        _pauseMenu.enabled = false;
        Time.timeScale = 1;
    }

    private void OnSaveClicked()
    {
        SaveService.Save();
        SaveService.SaveInventory();
        OnContinueClicked();
    }

    private void OnMainMenuClicked() => SceneManager.LoadScene(0);
    private void OnDisable()
    {
        _buttons[0].onClick.RemoveListener(OnContinueClicked);
        _buttons[1].onClick.RemoveListener(OnSaveClicked);
        _buttons[2].onClick.RemoveListener(OnMainMenuClicked);
    }
}
