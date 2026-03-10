using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboTextAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private float expandDuration = 0.25f;
    [SerializeField] private float holdDuration = 0.6f;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float bounceScale = 1.3f;

    private Sequence currentSequence;

    private void Awake()
    {
        comboText.alpha = 0f;
        comboText.transform.localScale = Vector3.one;
    }

    public void PlayCombo(int comboCount, int points)
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }

        comboText.text = $"COMBO X{(int)Mathf.Pow(2, comboCount - 1)}";
        comboText.alpha = 1f;
        comboText.transform.localScale = new Vector3(0f, 1f, 1f);

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            comboText.transform.DOScaleX(1f, expandDuration)
                .SetEase(Ease.OutBack)
        );

        currentSequence.Join(
            comboText.transform.DOScaleY(bounceScale, expandDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );

        currentSequence.AppendInterval(holdDuration);

        currentSequence.Append(
            comboText.DOFade(0f, fadeDuration)
                .SetEase(Ease.InQuad)
        );

        currentSequence.SetAutoKill(true);
    }

    public void BounceExisting()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }

        comboText.alpha = 1f;

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            comboText.transform.DOScale(bounceScale * 1.1f, 0.12f)
                .SetEase(Ease.OutQuad)
        );
        currentSequence.Append(
            comboText.transform.DOScale(1f, 0.15f)
                .SetEase(Ease.InQuad)
        );
        currentSequence.AppendInterval(holdDuration);
        currentSequence.Append(
            comboText.DOFade(0f, fadeDuration)
                .SetEase(Ease.InQuad)
        );

        currentSequence.SetAutoKill(true);
    }
}