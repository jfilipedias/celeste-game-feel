using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private float waitTimeToReload = 0.5f;

    private void Update()
    {
        if (playerGameObject == null)
            StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(waitTimeToReload);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
