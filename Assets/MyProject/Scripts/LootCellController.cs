using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class LootCellController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Sprite _startSprite;
    [SerializeField] private InventoryController _inventory;

    [HideInInspector] public ItemSO CurrentItem;
    [HideInInspector] public bool IsEmpty = true;
    [HideInInspector] public int Amount = 0;
    [HideInInspector] public bool CanUse;

    private Image _image;
    private TextMeshProUGUI _counter;
    private PlayerController _player;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _image = GetComponent<Image>();
        _counter = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Amount > 1) _counter.text = Amount.ToString();
        else _counter.text = string.Empty;

        if (Input.GetKeyDown(KeyCode.R) && !IsEmpty)
        {
            while (Amount > 0)
            {
                StaticData.OnItemSold?.Invoke(CurrentItem.Price);
                ClearCell();
            }
        }
    }
    public void SetItem(ItemSO item, int amount)
    {
        CurrentItem = item;
        IsEmpty = false;
        _image.sprite = CurrentItem.Icon;
        Amount = amount;
        CanUse = true;
    }

    public void ClearCell()
    {
        Amount -= 1;
        _gameManager.RemoveLoot(CurrentItem, Amount);
        if (Amount == 0)
        {
            CurrentItem = null;
            IsEmpty = true;
            _image.sprite = _startSprite;
        }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsEmpty)
        {
            if (CanUse)
            {
                switch (CurrentItem.Type)
                {
                    case ItemType.Cure:
                        _player.GetComponent<HPController>().Healing(CurrentItem.HealPercents);
                        break;
                    case ItemType.Weapon:
                        _inventory.SetWeapon(CurrentItem);
                        break;
                    case ItemType.Shield:
                        _inventory.SetShield(CurrentItem);
                        break;
                    case ItemType.Quiver:
                        _inventory.SetQuiver(CurrentItem);
                        break;
                    case ItemType.Armor:
                        _inventory.SetHelmet(CurrentItem);
                        break;
                    default: return;
                }
            }
            else StaticData.OnItemSold?.Invoke(CurrentItem.Price);
            ClearCell();
        }
    }
}
