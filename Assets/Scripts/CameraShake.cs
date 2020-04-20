using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float duration;

    public IEnumerator Shake()
    {
        Debug.Log("Shaking");

        Vector3 originalPosition = this.transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * force;
            float y = Random.Range(-1f, 1f) * force;

            this.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        this.transform.localPosition = originalPosition; 
    }
}
