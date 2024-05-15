using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LavaController : MonoBehaviour
{
    public TerrainLayer LavaLayer;
    public GameObject FireClone;
    [SerializeField] private float _damage;

    private GameObject _fireClone;
    private float _lavaMaxOffset = 5;
    private float _lavaOffsetCount = 0;
    private bool _isInside = false;

    void Update()
    {
        _lavaOffsetCount += Time.deltaTime;
        if (_lavaOffsetCount >= _lavaMaxOffset) _lavaOffsetCount = 0;
        LavaLayer.tileOffset = new Vector2(0, _lavaOffsetCount);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !_isInside)
        {
            _isInside = true;
            _fireClone = Instantiate(FireClone, other.gameObject.transform);
            Destroy(_fireClone, 15f);
            StartCoroutine(SetDamage(other.GetComponent<HPController>()));
        }
    }



    private IEnumerator SetDamage(HPController hp)
    {
        hp.SetDamage(_damage);
        yield return new WaitForSeconds(1);
        _isInside = false;
    }
}
