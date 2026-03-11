using UnityEngine;

public class Card : MonoBehaviour
{
    public int PairId { get; private set; }
    public bool IsMatched { get; private set; } = false;

    [SerializeField] private CardFlipper flipper;
    [SerializeField] private float flipDuration = 0.4f;
    [SerializeField] private float matchAnimDuration = 0.3f;

    public void Init(int pairId)
    {
        if (flipper == null)
            flipper = GetComponent<CardFlipper>();

        PairId = pairId;
        IsMatched = false;
        flipper.SetBackImmediate();
    }

    public bool IsFaceUp => flipper.IsFaceUp;

    public void FlipToFace()
    {
        if (IsMatched || IsFaceUp) return;
        flipper.FlipToFace(flipDuration);
    }

    public void FlipToBack()
    {
        if (IsMatched || !IsFaceUp) return;
        flipper.FlipToBack(flipDuration);
    }

    public void ShowFaceImmediate()
    {
        flipper.SetFaceImmediate();
    }

    public void PlayMatchAnimation(System.Action onComplete = null)
    {
        IsMatched = true;
        flipper.PlayMatchAnimation(matchAnimDuration, () =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private void OnMouseDown()
    {
        if (IsMatched || IsFaceUp) return;
        CardMatcher.Instance.OnCardTapped(this);
    }
}