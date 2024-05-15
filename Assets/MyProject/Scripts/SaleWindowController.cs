using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaleWindowController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _itemInfo;

    private ItemController _itemForSale;
    private Canvas _canvas;
    private bool _isActive = true;
    private void OnEnable()
    {
        _canvas = GetComponent<Canvas>();   
        _canvas.enabled = false;
        StaticData.OnItemInfoChanged += SetItemInfo;
        StaticData.DeactivateSaleWindow += Deactivate;
    }

    private void SetItemInfo(string info, ItemController item)
    {
        if (_isActive)
        {
            _canvas.enabled = (_canvas.enabled == false) ? true : false;
            _itemInfo.text = info;
            _itemForSale = item;
        }
    }


    public void OnReturnClick()
    {
        SetItemInfo(string.Empty, null);
        Deactivate(true);
    }

    public void OnPurchaseClick()
    {
        FindObjectOfType<InventoryController>().PurchaseItem(_itemForSale);
        SetItemInfo(string.Empty, null);
        Deactivate(true);
    }

    public void Deactivate(bool value) => _isActive = value;


    private void OnDisable()
    {
        StaticData.OnItemInfoChanged -= SetItemInfo;
        StaticData.DeactivateSaleWindow -= Deactivate;
    }
}
