using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StaticData.OnHintChanged?.Invoke("Безопасная зона : кристальное ущелье");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StaticData.OnHintChanged?.Invoke("Вы покидаете безопасную зону");
        }
    }
}
