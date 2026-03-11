using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardMatcher : MonoBehaviour
{
    public static CardMatcher Instance { get; private set; }

    [SerializeField] private float previewDuration = 3f;
    [SerializeField] private float spawnAnimDuration = 0.55f;
    [SerializeField] private float matchCheckDelay = 0.5f;

    private Card firstCard;
    private Card secondCard;
    private Card pendingMismatchA;
    private Card pendingMismatchB;
    private bool isCheckingMatch = false;
    private bool isLocked = false;
    private List<Card> allCards = new List<Card>();
    private int matchedPairs = 0;
    private int totalPairs = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel(List<Card> cards, int gridRows = 4, int gridCols = 4)
    {
        allCards = cards;
        firstCard = null;
        secondCard = null;
        pendingMismatchA = null;
        pendingMismatchB = null;
        isCheckingMatch = false;
        isLocked = false;
        matchedPairs = 0;
        totalPairs = cards.Count / 2;
        StartCoroutine(PreviewRoutine(gridRows, gridCols));
    }

    private IEnumerator PreviewRoutine(int gridRows, int gridCols)
    {
        isLocked = true;

        float lastCardDelay = (gridRows - 1) * 0.15f;
        float spawnFinishTime = lastCardDelay + spawnAnimDuration;
        yield return new WaitForSeconds(spawnFinishTime);

        yield return new WaitForSeconds(previewDuration);

        int flipped = 0;
        foreach (var card in allCards)
            card.FlipToBackWithJump(() => flipped++);

        yield return new WaitUntil(() => flipped >= allCards.Count);
        yield return new WaitForSeconds(0.1f);

        isLocked = false;
    }

    public void OnCardTapped(Card card)
    {
        if (isLocked) return;
        if (card.IsMatched) return;

        if (isCheckingMatch)
        {
            if (card == pendingMismatchA || card == pendingMismatchB) return;

            pendingMismatchA?.FlipToBack();
            pendingMismatchB?.FlipToBack();
            pendingMismatchA = null;
            pendingMismatchB = null;
            isCheckingMatch = false;

            firstCard = card;
            card.FlipToFace();
            return;
        }

        if (firstCard == null)
        {
            firstCard = card;
            card.FlipToFace();
            return;
        }

        if (firstCard == card)
        {
            card.Bounce();
            return;
        }

        secondCard = card;
        card.FlipToFace();
        StartCoroutine(CheckMatchRoutine());
    }

    private IEnumerator CheckMatchRoutine()
    {
        isCheckingMatch = true;

        Card a = firstCard;
        Card b = secondCard;
        firstCard = null;
        secondCard = null;

        if (a == null || b == null)
        {
            isCheckingMatch = false;
            yield break;
        }

        if (a.PairId == b.PairId)
        {
            isCheckingMatch = false;
            matchedPairs++;
            bool isLastPair = matchedPairs >= totalPairs;

            int comboBeforeMatch = ScoreManager.Instance.ComboCount;
            ScoreManager.Instance.RegisterMatch();
            int coins = Mathf.Clamp((int)Mathf.Pow(2, comboBeforeMatch), 1, 8);

            Vector3 mergePoint = (a.transform.position + b.transform.position) * 0.5f;

            float flipDelay = a.GetFlipper().FlipDuration * 0.5f;
            yield return new WaitForSeconds(flipDelay);

            a.GetFlipper().SnapToFace();
            b.GetFlipper().SnapToFace();

            if (CardMergeEffect.Instance == null) { Debug.LogError("CardMergeEffect not in scene!"); yield break; }
            CardMergeEffect.Instance.PlayMerge(a.transform, b.transform, coins, () =>
            {
                CoinFlyEffect.Instance.SpawnCoins(mergePoint, coins);
                Destroy(a.gameObject);
                Destroy(b.gameObject);

                if (isLastPair)
                {
                    SaveManager.Instance.SaveLevel(SaveManager.Instance.CurrentLevel + 1);
                    LevelCompleteUI.Instance.Show();
                }
            });

            isLocked = isLastPair;
        }
        else
        {
            pendingMismatchA = a;
            pendingMismatchB = b;

            yield return new WaitForSeconds(matchCheckDelay);

            if (pendingMismatchA != null)
            {
                ScoreManager.Instance.ResetCombo();
                pendingMismatchA?.FlipToBack();
                pendingMismatchB?.FlipToBack();
                pendingMismatchA = null;
                pendingMismatchB = null;
            }

            isCheckingMatch = false;
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        firstCard = null;
        secondCard = null;
        pendingMismatchA = null;
        pendingMismatchB = null;
        isCheckingMatch = false;
        isLocked = false;
        matchedPairs = 0;
        totalPairs = 0;
        allCards.Clear();
    }
}