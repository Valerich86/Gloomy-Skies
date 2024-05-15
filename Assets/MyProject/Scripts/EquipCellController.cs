using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipCellController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite _startSprite;

    [HideInInspector] public ItemSO CurrentItem;
    [HideInInspector] public bool IsEmpty = true;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetItem(ItemSO item)
    {
        CurrentItem = item;
        IsEmpty = false;
        _image.sprite = CurrentItem.Icon;
    }


    public void ClearCell()
    {
        CurrentItem = null;
        IsEmpty = true;
        _image.sprite = _startSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = Color.blue;
        if (!IsEmpty) StaticData.OnCellEnter?.Invoke(CurrentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = Color.white;
        if (!IsEmpty) StaticData.OnCellExit?.Invoke();
    }
}
