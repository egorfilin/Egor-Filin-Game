using UnityEngine;
using System.Collections;

public class CardFlipper : MonoBehaviour
{
    public bool IsFaceUp { get; private set; } = false;

    private Coroutine flipCoroutine;

    public void FlipToFace(float duration)
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(FlipRoutine(0f, 180f, duration, () => IsFaceUp = true));
    }

    public void FlipToBack(float duration)
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(FlipRoutine(180f, 0f, duration, () => IsFaceUp = false));
    }

    public void SetFaceImmediate()
    {
        IsFaceUp = true;
        transform.localEulerAngles = new Vector3(0f, 180f, 0f);
    }

    public void SetBackImmediate()
    {
        IsFaceUp = false;
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    public void PlayMatchAnimation(float duration)
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
        flipCoroutine = StartCoroutine(ScaleDownRoutine(duration));
    }

    private IEnumerator FlipRoutine(float fromY, float toY, float duration, System.Action onComplete)
    {
        float half = duration / 2f;
        float midY = fromY + (toY - fromY) / 2f;

        yield return RotateYRoutine(fromY, midY, half);
        yield return RotateYRoutine(midY, toY, half);

        transform.localEulerAngles = new Vector3(0f, toY, 0f);
        onComplete?.Invoke();
        flipCoroutine = null;
    }

    private IEnumerator RotateYRoutine(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            float angle = Mathf.Lerp(from, to, t);
            transform.localEulerAngles = new Vector3(0f, angle, 0f);
            yield return null;
        }
    }

    private IEnumerator ScaleDownRoutine(float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        gameObject.SetActive(false);
        flipCoroutine = null;
    }
}