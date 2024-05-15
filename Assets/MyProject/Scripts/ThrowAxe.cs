using UnityEngine;

public class ThrowAxe : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _damage = 50;
    private float _smooth = 2;
    private Transform _parent;
    private bool _flyingForward = true;
    private bool _flyingBack = false;
    private bool _closeToPlayer = false;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Invoke("MoveBack", 2f);
    }

    void Update()
    {
        if (_flyingForward) transform.Rotate(0, 0, 100);
        if (_flyingBack)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            Vector3 finalPosition = FindObjectOfType<WeaponHand>().transform.position; 
            transform.position = Vector3.Lerp(transform.position, finalPosition, _smooth * Time.deltaTime);
            transform.Rotate(-100, 0, 0);
            if (Vector3.Distance(transform.position, finalPosition) < 3 && !_closeToPlayer)
            {
                _closeToPlayer = true;
                player.GetComponent<Animator>().SetTrigger("AxeCatch");
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        _flyingForward = false;
        //_rigidbody.isKinematic = true;
        if (collision.gameObject.TryGetComponent<EnemyController>(out EnemyController enemy))
            enemy.GetComponent<HPController>().TakeDamage(_damage);
        MoveBack();
    }

    private void MoveBack()
    {
        _flyingForward = false;
        _flyingBack = true;
    }
}
