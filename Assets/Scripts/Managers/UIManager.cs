using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ComboTextAnimator comboAnimator;

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
        ScoreManager.Instance.OnCombo += OnCombo;
        UpdateScore(0);
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        ScoreManager.Instance.OnCombo -= OnCombo;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"{score}";
        scoreText.transform.DOKill();
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f);
    }

    private void OnCombo(int comboCount, int points)
    {
        comboAnimator.PlayCombo(comboCount, points);
    }
}