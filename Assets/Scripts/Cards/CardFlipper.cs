using UnityEngine;
using DG.Tweening;

public class CardFlipper : MonoBehaviour
{
    public bool IsFaceUp { get; private set; } = false;

    [SerializeField] private float flipDuration = 0.5f;

    public float FlipDuration => flipDuration;
    [SerializeField] private float jumpHeight = 0.5f;
    private Sequence currentSequence;
    private Vector3 originalScale;
    private Vector3 baseLocalPosition;
    private bool scaleInitialized = false;

    public void ForceInitScale()
    {
        originalScale = transform.localScale;
        baseLocalPosition = transform.localPosition;
        scaleInitialized = true;
    }

    private void CaptureScale()
    {
        if (!scaleInitialized)
        {
            originalScale = transform.localScale;
            baseLocalPosition = transform.localPosition;
            scaleInitialized = true;
        }
    }

    public void FlipToFace(System.Action onComplete = null)
    {
        CaptureScale();
        IsFaceUp = true;
        PlayFlip(0f, -180f, true, onComplete);
    }

    public void FlipToBack(System.Action onComplete = null, bool withBounce = true)
    {
        CaptureScale();
        IsFaceUp = false;
        PlayFlip(-180f, -360f, withBounce, onComplete);
    }

    public void FlipFull(System.Action onComplete = null)
    {
        CaptureScale();
        float currentX = transform.localEulerAngles.x;
        if (currentX > 180f) currentX -= 360f;
        PlayFlip(currentX, currentX - 360f, true, onComplete);
    }

    public void SetFaceImmediate()
    {
        CaptureScale();
        KillCurrent();
        IsFaceUp = true;
        transform.localEulerAngles = new Vector3(-180f, 0f, 0f);
        transform.localScale = originalScale;
        transform.localPosition = baseLocalPosition;
    }

    public void SetBackImmediate()
    {
        CaptureScale();
        KillCurrent();
        IsFaceUp = false;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = originalScale;
        transform.localPosition = baseLocalPosition;
    }

    public void PlayMatchAnimation(System.Action onComplete = null)
    {
        CaptureScale();
        KillCurrent();

        currentSequence = DOTween.Sequence();
        currentSequence.Append(transform.DOScale(originalScale * 1.15f, 0.1f).SetEase(Ease.OutQuad));
        currentSequence.Append(transform.DOScale(Vector3.zero, 0.22f).SetEase(Ease.InBack));
        currentSequence.OnComplete(() => onComplete?.Invoke());
    }

    public void PlaySpawnAnimation(float delay, bool faceUp = false)
    {
        CaptureScale();
        KillCurrent();
        transform.localScale = Vector3.zero;
        transform.localPosition = baseLocalPosition;

        IsFaceUp = faceUp;
        transform.localEulerAngles = faceUp ? new Vector3(-180f, 0f, 0f) : Vector3.zero;

        currentSequence = DOTween.Sequence();
        currentSequence.AppendInterval(delay);
        currentSequence.Append(transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack));
    }

    public void PlayBounce()
    {
        CaptureScale();
        KillCurrent();

        float b = baseLocalPosition.z;
        currentSequence = DOTween.Sequence();
        currentSequence.Append(transform.DOLocalMoveZ(b - jumpHeight, 0.1f).SetEase(Ease.OutQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b, 0.1f).SetEase(Ease.InQuad));
        currentSequence.OnComplete(() =>
        {
            transform.localPosition = baseLocalPosition;
            currentSequence = null;
        });
    }

    public void FlipToBackWithJump(System.Action onComplete = null)
    {
        CaptureScale();
        IsFaceUp = false;

        float b = baseLocalPosition.z;
        float spawnJump = 3f;
        float dur = 0.3f;
        float fromX = -180f;
        float toX = -360f;

        KillCurrent();

        currentSequence = DOTween.Sequence();
        currentSequence.Append(transform.DOLocalMoveZ(b - spawnJump, dur * 0.4f).SetEase(Ease.OutQuad));
        currentSequence.Join(
            DOTween.To(
                () => fromX,
                x => transform.localEulerAngles = new Vector3(x, 0f, 0f),
                toX,
                dur
            ).SetEase(Ease.InOutQuad)
        );
        currentSequence.Append(transform.DOLocalMoveZ(b, dur * 0.3f).SetEase(Ease.InQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b - spawnJump * 0.2f, dur * 0.15f).SetEase(Ease.OutQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b, dur * 0.15f).SetEase(Ease.InQuad));
        currentSequence.OnComplete(() =>
        {
            transform.localPosition = baseLocalPosition;
            transform.localScale = originalScale;
            onComplete?.Invoke();
        });
    }

    public void KillFlip()
    {
        KillCurrent();
    }

    public void SnapToFace()
    {
        KillCurrent();
        IsFaceUp = true;
        transform.localEulerAngles = new Vector3(-180f, 0f, 0f);
        transform.localScale = originalScale;
        transform.localPosition = baseLocalPosition;
    }

    private void PlayFlip(float fromX, float toX, bool withBounce, System.Action onComplete)
    {
        KillCurrent();

        float b = baseLocalPosition.z;

        if (!withBounce)
        {
            currentSequence = DOTween.Sequence();
            currentSequence.Append(
                DOTween.To(
                    () => fromX,
                    x => transform.localEulerAngles = new Vector3(x, 0f, 0f),
                    toX,
                    flipDuration * 0.7f
                ).SetEase(Ease.InOutQuad)
            );
            currentSequence.OnComplete(() =>
            {
                transform.localPosition = baseLocalPosition;
                transform.localScale = originalScale;
                onComplete?.Invoke();
            });
            return;
        }

        float upTime   = flipDuration * 0.4f;
        float downTime = flipDuration * 0.3f;
        float b1Up     = flipDuration * 0.12f;
        float b1Down   = flipDuration * 0.10f;
        float b2Up     = flipDuration * 0.05f;
        float b2Down   = flipDuration * 0.03f;

        currentSequence = DOTween.Sequence();

        currentSequence.Append(transform.DOLocalMoveZ(b - jumpHeight, upTime).SetEase(Ease.OutQuad));
        currentSequence.Join(
            DOTween.To(
                () => fromX,
                x => transform.localEulerAngles = new Vector3(x, 0f, 0f),
                toX,
                upTime + downTime
            ).SetEase(Ease.InOutQuad)
        );
        currentSequence.Append(transform.DOLocalMoveZ(b, downTime).SetEase(Ease.InQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b - jumpHeight * 0.28f, b1Up).SetEase(Ease.OutQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b, b1Down).SetEase(Ease.InQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b - jumpHeight * 0.08f, b2Up).SetEase(Ease.OutQuad));
        currentSequence.Append(transform.DOLocalMoveZ(b, b2Down).SetEase(Ease.InQuad));

        currentSequence.OnComplete(() =>
        {
            transform.localPosition = baseLocalPosition;
            transform.localScale = originalScale;
            onComplete?.Invoke();
        });
    }

    private void KillCurrent()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }
        if (scaleInitialized)
        {
            transform.localPosition = baseLocalPosition;
            transform.localScale = originalScale;
        }
    }
}