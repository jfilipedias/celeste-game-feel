using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private List<string> additiveSceneList = new List<string>();
    [SerializeField] private GameObject playerGameObject;

    private void Awake()
    {
        foreach (string scene in additiveSceneList)
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (playerGameObject == null)
            StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
