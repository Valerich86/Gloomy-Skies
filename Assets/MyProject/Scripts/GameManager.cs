using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public Role[] Roles;
    public ItemSO[] Items;
    public Role CurrentRole { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentMoney { get; private set; }
    public float CurrentExperience { get; private set; }
    public float MaxExperience { get; private set; }
    public float CurrentHP { get; private set; }
    public float MaxHP { get; private set; }

    [HideInInspector] public Dictionary<ItemSO, int> CurrentLoot = new();

    [HideInInspector] public Dictionary<ItemSO, int> CurrentEquip = new();

    private PlayerController _player;

    private static GameManager instance = null;

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentMoney = 1000;
        CurrentLevel = 1;

        SetCurrentExperience(SaveService.Restore().Exp, SaveService.Restore().MaxExp);
    }
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Player"))
        {
            SaveService.IsLoading = true;
            CurrentRole = Roles[SaveService.Restore().Role];
            CurrentMoney = SaveService.Restore().Money;
            CurrentLevel = SaveService.Restore().Level;
            SetCurrenrHP(SaveService.Restore().HP, SaveService.Restore().MaxHP);
            SceneManager.LoadScene(SaveService.Restore().Scene);
            StartCoroutine (Convert_int_to_SO(SaveService.LoadInventory()));
            SetCurrentExperience(SaveService.Restore().Exp, SaveService.Restore().MaxExp);
        }
    }

    private IEnumerator Convert_int_to_SO(InventoryData data)
    {
        yield return new WaitForSeconds(1);
        CurrentEquip.Clear();
        CurrentLoot.Clear();
        InventoryController _inventory = FindObjectOfType<InventoryController>();
        foreach (KeyValuePair<int, int> values in data.Equip)
        {
            foreach (var item in Items)
            {
                if (values.Key == item.ID)
                {
                    AddEquip(item, values.Value);
                }
            }
        }
        foreach (KeyValuePair<int, int> values in data.Loot)
        {
            foreach (var item in Items)
            {
                if (values.Key == item.ID)
                {
                    AddLoot(item);
                }
            }
        }
        _inventory.SetInventory();
        _inventory.PutOn();
    }
    public PlayerController SetPlayerToSave()
    {
        _player = FindObjectOfType<PlayerController>();
        return _player;
    }
    public void SetCurrentRole(Role role)
    {
        CurrentRole = role;
        CurrentHP = role.MaxHP;
        MaxHP = role.MaxHP;
        SetStartEquipment();
    }
    private void SetStartEquipment()
    {
        if (CurrentRole.Weapon != null) AddEquip(CurrentRole.Weapon, 0);
        if (CurrentRole.Shield != null) AddEquip(CurrentRole.Shield, 1);
    }
    public void SetCurrentLevel(int level) => CurrentLevel = level;
    public void SetCurrentMoney(int money) => CurrentMoney += money;
    public void RemoveEquip(ItemSO item) => CurrentEquip.Remove(item);
    public void SetCurrentExperience(float exp, float max)
    {
        CurrentExperience = exp;
        MaxExperience = max;
    }
    public void SetCurrenrHP(float hp, float max)
    {
        CurrentHP = hp;
        MaxHP = max;
    }
    public void AddEquip(ItemSO item, int cellNum)
    {
        try
        {
            CurrentEquip.Add(item, cellNum);
        }
        catch (Exception)
        {
            return;
        }
    }
    public void AddLoot(ItemSO item)
    {
        try
        {
            CurrentLoot.Add(item, 1);
        }
        catch (Exception)
        {
            CurrentLoot[item] += 1;
        }
    }
    public void RemoveLoot(ItemSO item, int amount)
    {
        foreach (KeyValuePair<ItemSO, int> _item in CurrentLoot)    
        {
            if (item == _item.Key)
            {
                if (amount == 0)
                {
                    CurrentLoot.Remove(item);
                    return;
                } 
                CurrentLoot[_item.Key] = amount;
            }
        }
    }

}
