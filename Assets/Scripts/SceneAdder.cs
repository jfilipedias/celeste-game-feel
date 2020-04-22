using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAdder : MonoBehaviour
{
    [SerializeField] private List<string> additiveSceneList = new List<string>();

    private void Awake()
    {
        foreach (string scene in additiveSceneList)
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
}
