using UnityEngine;

public class Card : MonoBehaviour
{
    public int PairId { get; private set; }
    public bool IsMatched { get; private set; } = false;

    [SerializeField] private CardFlipper flipper;

    public void Init(int pairId)
    {
        if (flipper == null)
            flipper = GetComponent<CardFlipper>();

        PairId = pairId;
        IsMatched = false;
        flipper.ForceInitScale();
        flipper.SetBackImmediate();
    }

    public bool IsFaceUp => flipper.IsFaceUp;

    public CardFlipper GetFlipper() => flipper;

    public void Bounce()
    {
        if (IsMatched) return;
        flipper.PlayBounce();
    }

    public void FlipToFace(System.Action onComplete = null)
    {
        if (IsMatched) return;
        flipper.FlipToFace(onComplete);
    }

    public void FlipToBack(System.Action onComplete = null, bool withBounce = true)
    {
        if (IsMatched) return;
        flipper.FlipToBack(onComplete, withBounce);
    }

    public void FlipToBackWithJump(System.Action onComplete = null)
    {
        if (IsMatched) return;
        flipper.FlipToBackWithJump(onComplete);
    }

    public void FlipFull(System.Action onComplete = null)
    {
        if (IsMatched) return;
        flipper.FlipFull(onComplete);
    }

    public void ShowFaceImmediate()
    {
        flipper.SetFaceImmediate();
    }

    public void PlaySpawnAnimation(float delay)
    {
        flipper.PlaySpawnAnimation(delay, faceUp: true);
    }

    public void SetSpawnDelay(int row, int col)
    {
        float delay = row * 0.15f;
        flipper.PlaySpawnAnimation(delay, faceUp: true);
    }

    public void PlayMatchAnimation(System.Action onComplete = null)
    {
        IsMatched = true;
        flipper.PlayMatchAnimation(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private void OnMouseDown()
    {
        if (IsMatched) return;
        CardMatcher.Instance.OnCardTapped(this);
    }
}