using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private TextMeshProUGUI _text;

    private int _nextSceneID;
    AsyncOperation _asyncOp;

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        _nextSceneID = scene.buildIndex + 1;
        StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        yield return new WaitForSeconds(1f);
        _asyncOp = SceneManager.LoadSceneAsync(_nextSceneID);
        while (!_asyncOp.isDone)
        {
            float progress = _asyncOp.progress / 0.9f;
            _bar.fillAmount = progress;
            _text.text = "Загрузка " + string.Format("{0:0}%", progress * 100f);
            yield return null;
        }
    }
}
