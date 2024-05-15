using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTeleport : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SaveService.IsLoading = false;
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 2) SceneManager.LoadScene(3);
        else if (scene.buildIndex == 4) SceneManager.LoadScene(1);
    }
}
