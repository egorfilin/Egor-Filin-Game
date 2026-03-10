using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardMatcher : MonoBehaviour
{
    public static CardMatcher Instance { get; private set; }

    [SerializeField] private float previewDuration = 3f;
    [SerializeField] private float mismatchDelay = 0.8f;

    private Card firstCard;
    private bool isLocked = false;
    private List<Card> allCards = new List<Card>();

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel(List<Card> cards)
    {
        allCards = cards;
        firstCard = null;
        isLocked = false;
        StartCoroutine(PreviewRoutine());
    }

    private IEnumerator PreviewRoutine()
    {
        isLocked = true;

        foreach (var card in allCards)
            card.ShowFaceImmediate();

        yield return new WaitForSeconds(previewDuration);

        foreach (var card in allCards)
            card.FlipToBack();

        yield return new WaitForSeconds(0.5f);

        isLocked = false;
    }

    public void OnCardTapped(Card card)
    {
        if (isLocked) return;

        if (firstCard == null)
        {
            firstCard = card;
            card.FlipToFace();
            return;
        }

        if (firstCard == card) return;

        card.FlipToFace();
        isLocked = true;
        StartCoroutine(CheckMatchRoutine(firstCard, card));
        firstCard = null;
    }

    private IEnumerator CheckMatchRoutine(Card a, Card b)
    {
        yield return new WaitForSeconds(0.5f);

        if (a.PairId == b.PairId)
        {
            a.PlayMatchAnimation();
            b.PlayMatchAnimation();
            ScoreManager.Instance.RegisterMatch();
        }
        else
        {
            ScoreManager.Instance.ResetCombo();
            yield return new WaitForSeconds(mismatchDelay);
            a.FlipToBack();
            b.FlipToBack();
        }

        isLocked = false;
    }

    public void Reset()
    {
        StopAllCoroutines();
        firstCard = null;
        isLocked = false;
        allCards.Clear();
    }
}