using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public enum ItemAssignment { Equipment, Loot, Money, SetOfArrows }
public enum ItemType { Weapon, Armor, Shield, Quiver, Ability, Cure, Key, ForSale, Other }
public class InventoryController : MonoBehaviour
{

    [SerializeField] private EquipCellController[] _equipCells;
    [SerializeField] private LootCellController[] _lootCells;
    [SerializeField] private TextMeshProUGUI[] _descriptions;
    [SerializeField] private TextMeshProUGUI _moneyCounter;
    [SerializeField] private GameObject _arrow;
    [HideInInspector] public int ArrowAmount { get; private set; } = 20;
    [HideInInspector] public int MaxArrowAmount { get; private set; } = 20;

    private GameManager _gameManager;
    private PlayerController _playerController;

    public void Construct(PlayerController player)
    {
        _gameManager = GameManager.Instance;
        _playerController = player;
        SetMoney(0);
        StaticData.OnItemPicked += CheckCell;
        StaticData.OnItemSold += SetMoney;
        StaticData.OnCellEnter += SetDescriptions;
        StaticData.OnCellExit += ClearDescriptions;
        StaticData.OnArrowAmountChanged += SetCurrentArrowAmount;
        _arrow.SetActive(false);
        gameObject.SetActive(true);
        ClearDescriptions();
    }


    public void SetCurrentArrowAmount(int amount)
    {
        ArrowAmount += amount;
        if (ArrowAmount > MaxArrowAmount) ArrowAmount = MaxArrowAmount;
        _arrow.GetComponentInChildren<TextMeshProUGUI>().text = $"{ArrowAmount} / {MaxArrowAmount}";
        _playerController.GetComponent<AttackController>().SetCurrentArrowsAmount(ArrowAmount);
    }


    private void ClearDescriptions()
    {
        foreach (var text in _descriptions)
        {
            text.text = string.Empty;
        }
    }

    private void SetDescriptions(ItemSO item)
    {
        _descriptions[0].text = $"'{item.Tytle}'";
        _descriptions[1].text = item.Description;
        _descriptions[2].text = $"Стоимость : {item.Price}";
    }

    public void SetInventory()
    {
        if (_gameManager.CurrentLoot.Count > 0)
        {
            int cell = 0;
            foreach (KeyValuePair<ItemSO, int> item in _gameManager.CurrentLoot)
            {
                _lootCells[cell].SetItem(item.Key, item.Value);
                cell++;
            }
        }
        if (_gameManager.CurrentEquip.Count > 0)
        {
            foreach (KeyValuePair<ItemSO, int> item in _gameManager.CurrentEquip)
            {
                _equipCells[item.Value].SetItem(item.Key);
            }
        }
    }

    public void PutOn()
    {
        if (_gameManager.CurrentEquip.Count > 0)
        {
            foreach (KeyValuePair<ItemSO, int> item in _gameManager.CurrentEquip)
            {
                Debug.Log($"Надето {item.Key} : {item.Value}");
                switch (item.Value)
                {
                    case 0:
                        _playerController.gameObject.GetComponent<AttackController>().SetPlayerWeapon(item.Key);
                        if (item.Key.WeaponSO.Type == AttackType.long_range)
                        {
                            _arrow.SetActive(true);
                            SetCurrentArrowAmount(ArrowAmount); 
                        }
                        break;
                    case 1:
                        _playerController.gameObject.GetComponent<AttackController>().SetPlayerShield(item.Key);
                        break;
                    case 2:
                        _playerController.gameObject.GetComponent<AttackController>().SetPlayerHelmet(item.Key);
                        break;
                    case 3:
                        MaxArrowAmount *= 2;
                        _playerController.gameObject.GetComponent<AttackController>().SetPlayerQuiver(item.Key);
                        break;
                    default: return;
                }
            }
        }
    }

    public void ChangeStatus(bool canUse)
    {
        foreach (var cell in _lootCells)
        {
            if (cell.IsEmpty) continue;
            cell.CanUse = canUse;
        }
    }

    private void CheckCell(ItemController item)
    {
        if (item.ItemSO.Assignment == ItemAssignment.Money)
        {
            StaticData.OnHintChanged?.Invoke($"Вы подобрали '{item.ItemSO.Tytle}'");
            Destroy(item.gameObject);
            SetMoney(item.ItemSO.Price);
            return;
        }
        StaticData.OnHintChanged?.Invoke($"Вы приобрели предмет '{item.ItemSO.Tytle}'");
        Destroy(item.gameObject);
        _gameManager.AddLoot(item.ItemSO);
        SetInventory();
    }

    public void SetWeapon(ItemSO weapon)
    {
        ItemSO currentItem;
        if (!_equipCells[0].IsEmpty)
        {
            currentItem = _equipCells[0].CurrentItem;
            _equipCells[0].SetItem(weapon);
            _gameManager.AddLoot(currentItem);
            _gameManager.RemoveEquip(currentItem);
            _gameManager.AddEquip(weapon, 0);
            SetInventory();
        }
        else
        {
            _equipCells[0].SetItem(weapon);
            _gameManager.AddEquip(weapon, 0);
        }
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerWeapon(weapon);
        if (weapon.WeaponSO.Type == AttackType.long_range)
        {
            _arrow.SetActive(true);
            _playerController.gameObject.GetComponent<AttackController>().SetCurrentArrowsAmount(ArrowAmount);
        }
    }

    public void SetShield(ItemSO shield)
    {
        ItemSO currentItem;
        if (!_equipCells[1].IsEmpty)
        {
            currentItem = _equipCells[1].CurrentItem;
            _equipCells[1].SetItem(shield);
            _gameManager.AddLoot(currentItem);
            _gameManager.RemoveEquip(currentItem);
            _gameManager.AddEquip(shield, 1);
            SetInventory();
        }
        else
        {
            _equipCells[1].SetItem(shield);
            _gameManager.AddEquip(shield, 1);
        }
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerShield(shield);
    }

    public void SetHelmet(ItemSO helmet)
    {
        ItemSO currentItem;
        if (!_equipCells[2].IsEmpty)
        {
            currentItem = _equipCells[2].CurrentItem;
            _equipCells[2].SetItem(helmet);
            _gameManager.AddLoot(currentItem);
            _gameManager.RemoveEquip(currentItem);
            _gameManager.AddEquip(helmet, 2);
            SetInventory();
        }
        else
        {
            _equipCells[2].SetItem(helmet);
            _gameManager.AddEquip(helmet, 2);
        }
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerHelmet(helmet);
    }

    public void SetQuiver(ItemSO quiver)
    {
        ItemSO currentItem;
        if (!_equipCells[3].IsEmpty)
        {
            currentItem = _equipCells[3].CurrentItem;
            _equipCells[3].SetItem(quiver);
            _gameManager.AddLoot(currentItem);
            _gameManager.RemoveEquip(currentItem);
            _gameManager.AddEquip(quiver, 3);
            SetInventory();
        }
        else
        {
            _equipCells[3].SetItem(quiver);
            _gameManager.AddEquip(quiver, 3);
        }
        MaxArrowAmount *= 2;
        _playerController.gameObject.GetComponent<AttackController>().SetPlayerQuiver(quiver);
    }

    private void SetMoney(int money)
    {
        _gameManager.SetCurrentMoney(money);
        _moneyCounter.text = _gameManager.CurrentMoney.ToString();
    }

    private IEnumerator ShowSomeHint(string hint, float time)
    {
        yield return new WaitForSeconds(time);
        StaticData.OnGlobalHintChanged?.Invoke(hint, 10);
    }

    public void PurchaseItem(ItemController item)
    {
        if (item.ItemSO.Price > _gameManager.CurrentMoney)
        {
            StaticData.OnHintChanged("Недостаточно денег");
            return;
        }
        else
        {
            SetMoney(-item.ItemSO.Price);
            CheckCell(item);
        }
    }

    private void OnDisable()
    {
        StaticData.OnItemPicked -= CheckCell;
        StaticData.OnItemSold -= SetMoney;
        StaticData.OnCellEnter -= SetDescriptions;
        StaticData.OnCellExit -= ClearDescriptions;
        StaticData.OnArrowAmountChanged -= SetCurrentArrowAmount;
    }

}
