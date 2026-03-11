using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CardMergeEffect : MonoBehaviour
{
    public static CardMergeEffect Instance { get; private set; }

    [SerializeField] private GameObject mergeParticlePrefab;
    [SerializeField] private float liftHeight = 0.5f;
    [SerializeField] private float liftDuration = 0.2f;
    [SerializeField] private float mergeDuration = 0.25f;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayMerge(Transform cardA, Transform cardB, int coins, System.Action onComplete = null)
    {
        StartCoroutine(MergeRoutine(cardA, cardB, coins, onComplete));
    }

    private IEnumerator MergeRoutine(Transform a, Transform b, int coins, System.Action onComplete)
    {
        Vector3 midPoint = (a.position + b.position) * 0.5f;

        Sequence liftA = DOTween.Sequence();
        liftA.Append(a.DOMove(a.position - Vector3.forward * liftHeight, liftDuration).SetEase(Ease.OutQuad));

        Sequence liftB = DOTween.Sequence();
        liftB.Append(b.DOMove(b.position - Vector3.forward * liftHeight, liftDuration).SetEase(Ease.OutQuad));

        yield return new WaitForSeconds(liftDuration);

        a.DOMove(midPoint - Vector3.forward * liftHeight, mergeDuration).SetEase(Ease.InQuad);
        b.DOMove(midPoint - Vector3.forward * liftHeight, mergeDuration).SetEase(Ease.InQuad);

        a.DOScale(Vector3.zero, mergeDuration).SetEase(Ease.InBack);
        b.DOScale(Vector3.zero, mergeDuration).SetEase(Ease.InBack);

        yield return new WaitForSeconds(mergeDuration);

        if (mergeParticlePrefab != null)
        {
            GameObject fx = Instantiate(mergeParticlePrefab, midPoint, Quaternion.identity);
            Destroy(fx, 2f);
        }

        onComplete?.Invoke();
    }
}