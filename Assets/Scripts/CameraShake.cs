using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public IEnumerator ShakeStart(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float actualDuration = 0.0f;

        while (actualDuration < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            actualDuration += Time.deltaTime;
            yield return new WaitForSeconds(0.025f);
        }

        transform.localPosition = originalPosition;
    }
}
