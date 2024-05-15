

using System;
using UnityEngine;

public enum RoleType { Rogue, HunterGirl, Blacksmith}
public static class StaticData 
{
    public static Action<string> OnHintChanged;
    public static Action<string, int> OnGlobalHintChanged;
    public static Action<string, ItemController> OnItemInfoChanged;
    public static Action<ItemController> OnItemPicked;
    public static Action<ItemSO> OnCellEnter;
    public static Action OnCellExit;
    public static Action OnSuperStrikeApplied;
    public static Action<EnemyController> OnEnemyDying;
    public static Action<int> OnArrowAmountChanged;
    public static Action<int> OnItemSold;
    public static Action<int, int> OnCameraChanged;
    public static Action<bool> DeactivateSaleWindow;
    public static Action OnPlayerDying;
}
