using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StaticData.OnHintChanged?.Invoke("Безопасная зона : оружейная лавка");
        }
    }

}
