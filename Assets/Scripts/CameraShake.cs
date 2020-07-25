using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float force = 0.1f;
    [SerializeField] private float duration = 0.1f;

    public async Task Shake()
    {
        Vector3 originalPosition = this.transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * force;
            float y = Random.Range(-1f, 1f) * force;

            this.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            await Task.Yield();
        }

        this.transform.localPosition = originalPosition; 
    }
}
