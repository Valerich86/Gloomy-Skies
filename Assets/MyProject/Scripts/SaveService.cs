using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class SaveService
{
    public static Vector3 PlayerPosition { get; private set; }
    public static bool IsLoading = false;

    private static GameManager _gameManager;
    private static PlayerController _player;
    private static Scene _scene;
    private static Vector3 _position;
    private static string _path = Application.persistentDataPath + "/InvSaveData.dat";
    public static void Save()
    {
        _gameManager = GameManager.Instance;
        _scene = SceneManager.GetActiveScene();
        _player = _gameManager.SetPlayerToSave();
        _position = _player.transform.position;
        SaveData saveData = new SaveData
        {
            Role = (int)_gameManager.CurrentRole.Type,
            Scene = _scene.buildIndex,
            Money = _gameManager.CurrentMoney,
            Level = _gameManager.CurrentLevel,
            Exp = _gameManager.CurrentExperience,
            MaxExp = _gameManager.MaxExperience,
            HP = _gameManager.CurrentHP,
            MaxHP = _gameManager.MaxHP,
            X = _position.x,
            Y = _position.y,
            Z = _position.z
        };
        string data = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("Player", data);
    }

    public static SaveData Restore()
    {
        if (PlayerPrefs.HasKey("Player"))
        {
            string json = PlayerPrefs.GetString("Player");
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData();
        }
    }

    public static void SaveInventory()
    {
        using (FileStream stream = new FileStream(_path, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            InventoryData data = Convert_SO_in_int();
            bf.Serialize(stream, data);
        }
        Debug.Log("Инвентарь сохранен");
    }

    public static InventoryData LoadInventory()
    {
        if (File.Exists(_path))
        {
            using (FileStream stream = new FileStream(_path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                InventoryData data = (InventoryData)bf.Deserialize(stream);
                Debug.Log("Инвентарь загружен");
                return data;
            }
        }
        else return new InventoryData();
    }

    public static void ResetInventory()
    {
        if (File.Exists(Application.persistentDataPath + "/InvSaveData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/InvSaveData.dat");
            Debug.Log("Инвентарь сброшен");
        }
    }

    public static Vector3 StartPosition()
    {
        return new Vector3(Restore().X, Restore().Y, Restore().Z);
    }

    private static Role CheckRole(int roleIndex)
    {
        return _gameManager.Roles[roleIndex];
    }

    private static InventoryData Convert_SO_in_int()
    {
        InventoryData data = new InventoryData();
        foreach(KeyValuePair<ItemSO, int> item in _gameManager.CurrentEquip)
        {
            data.Equip.Add(item.Key.ID, item.Value);
        }
        foreach (KeyValuePair<ItemSO, int> item in _gameManager.CurrentLoot)
        {
            data.Loot.Add(item.Key.ID, item.Value);
        }
        return data;
    }

}



[Serializable]
public class SaveData 
{
    public int Role = 4;
    public int Scene = 0;
    public int Level = 1;
    public int Money = 1000;
    public float Exp = 0;
    public float MaxExp = 500;
    public float HP;
    public float MaxHP;
    public float X;
    public float Y;
    public float Z;
}


[Serializable]
public class InventoryData
{
    public Dictionary<int, int> Equip = new();
    public Dictionary<int, int> Loot = new();
}


