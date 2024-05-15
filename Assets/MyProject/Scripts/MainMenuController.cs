using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _characterWindow;
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _quit;
    [SerializeField] private List<RoleButton> _roleButtons;

    private void OnEnable()
    {
        _characterWindow.SetActive(false);
        _startGame.onClick.AddListener(StartClicked);
        _resume.onClick.AddListener(ResumeClicked);
        _quit.onClick.AddListener(QuitClicked);
        foreach (var button in _roleButtons)
        {
            button.Init();
            button.OnClicked += OnClicked;
        }
    }

    private void ResumeClicked() => GameManager.Instance.LoadGame();

    private void OnClicked(Role role)
    {
        GameManager.Instance.SetCurrentRole(role);
        SceneManager.LoadScene(1);
    }

    private void StartClicked()
    {
        _characterWindow.SetActive(true);
        PlayerPrefs.DeleteAll();
        SaveService.ResetInventory();
        SaveService.IsLoading = false;
    }

    private void QuitClicked() => Application.Quit();

    private void OnDisable()
    {
        _startGame.onClick.RemoveListener(StartClicked);
        _resume.onClick.RemoveListener(ResumeClicked);
        _quit.onClick.RemoveListener(QuitClicked);
        foreach (var button in _roleButtons)
        {
            button.UnInit();
            button.OnClicked -= OnClicked;
        }
    }
}

[Serializable]
public class RoleButton
{
    public event Action<Role> OnClicked;
    public Role Role;
    public Button Button;

    public void Init() => Button.onClick.AddListener(OnButtonClicked);
    private void OnButtonClicked() => OnClicked?.Invoke(Role);
    public void UnInit() => Button.onClick.RemoveListener(OnButtonClicked);
}
