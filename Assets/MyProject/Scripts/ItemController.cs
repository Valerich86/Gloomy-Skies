using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemSO ItemSO;
    public bool ForSale;

    private bool _isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (_isActive && other.CompareTag("Player")) 
            StaticData.OnItemPicked?.Invoke(this);
    }

    public void Deactivate() => _isActive = false;


    public void OnMouseDown()
    {
        if (ForSale) StaticData.OnItemInfoChanged?.Invoke($"'{ItemSO.Tytle}'. {ItemSO.Description}. Стоимость: {ItemSO.Price}", this);
    }

}
