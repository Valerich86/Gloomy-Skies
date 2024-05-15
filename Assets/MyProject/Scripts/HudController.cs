using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image _healthbar;
    [SerializeField] private Image _abilityBar;
    [SerializeField] private Image _expBar;
    [SerializeField] private Image _icon;
    [SerializeField] private Canvas _inventoryWindow;
    [SerializeField] private TextMeshProUGUI _hint;
    [SerializeField] private TextMeshProUGUI _globalHint;
    [SerializeField] private TextMeshProUGUI _abilityText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private TextMeshProUGUI _hpText;

    private GameManager _gameManager;
    private HPController _hpController;
    private float _maxExperience;
    private float _maxAbility = 50;
    private float _minAbility = 30;
    private float _experience;
    private float _ability;
    private int _level;

    public void Construct(HPController hp, Sprite icon)
    {
        _gameManager = GameManager.Instance;
        _inventoryWindow.enabled = false;
        _hpController = hp;
        _hpController.OnHealthChanged += OnHealthChanged;
        StaticData.OnHintChanged += SetHint;
        StaticData.OnGlobalHintChanged += SetGlobalHint;
        StaticData.OnEnemyDying += OnExpChanged;
        StaticData.OnSuperStrikeApplied += ResetAbility;
        SetHint(String.Empty);
        SetGlobalHint(string.Empty, 1);
        _icon.sprite = icon;
        _expBar.fillAmount = 0;
        _abilityBar.fillAmount = 0;
        _level = _gameManager.CurrentLevel;
        _experience = _gameManager.CurrentExperience;
        _maxExperience = _gameManager.MaxExperience;
        _expText.text = "Опыт : уровень " + _level;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_inventoryWindow.enabled)
            {
                _inventoryWindow.enabled = true;
                Time.timeScale = 0;
                StaticData.DeactivateSaleWindow?.Invoke(false);
            }
            else
            {
                _inventoryWindow.enabled = false;
                Time.timeScale = 1;
                StaticData.DeactivateSaleWindow?.Invoke(true);
            }
        }
        
        if (_ability < _maxAbility)
        {
            _ability += Time.deltaTime;
            _abilityText.text = "Суперспособность";
        }
        else
        {
            _ability = _maxAbility;
            _abilityText.text = "' Q ' - активировать";
            FindObjectOfType<PlayerController>().ApplySS(true);
        } 
        _abilityBar.fillAmount = _ability / _maxAbility;
    }

    private void ResetAbility() => _ability = 0;

    private void OnHealthChanged(HPController health)
    {
        _healthbar.fillAmount = health.CurrentHP / health.MaxHP;
        _gameManager.SetCurrenrHP(health.CurrentHP, health.MaxHP);
    }
    private void OnExpChanged(EnemyController enemy)
    {
        _experience += enemy.Enemy.Exp;
        SetGlobalHint($"+ {enemy.Enemy.Exp} опыта !", 3);
        _expBar.fillAmount = _experience / _maxExperience;
        if (_experience >= _maxExperience) LevelUp();
        _expText.text = "Опыт : уровень " + _level;
        _gameManager.SetCurrentExperience(_experience, _maxExperience);
    }


    private void LevelUp()
    {
        _experience = 0;
        _level++;
        _maxExperience = _level * 500;
        _maxAbility = 60 - _level * 10;
        if (_maxAbility <= _minAbility) _maxAbility = _minAbility;
        if (_level > 1) _hpController.ChangeMaxHP(_level * 20);
        _gameManager.SetCurrentLevel(_level);
        _gameManager.SetCurrentExperience(_experience, _maxExperience);
    }

    public void SetGlobalHint(string hint, int time)
    {
        _globalHint.text = hint;
        Invoke("ClearGlobalHint", time);
    }

    public void SetHint(string hint)
    {
        _hint.text = hint;
        Invoke("ClearHint", 3f);
    }


    public void ClearHint() => _hint.text = string.Empty;

    public void ClearGlobalHint() => _globalHint.text = string.Empty;

    private void OnDisable()
    {
        StaticData.OnHintChanged -= SetHint;
        StaticData.OnGlobalHintChanged -= SetGlobalHint;
        _hpController.OnHealthChanged -= OnHealthChanged;
        StaticData.OnEnemyDying -= OnExpChanged;
        StaticData.OnSuperStrikeApplied -= ResetAbility;
    }
}
