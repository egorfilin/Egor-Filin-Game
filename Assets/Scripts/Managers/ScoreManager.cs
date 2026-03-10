using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int TotalScore { get; private set; } = 0;
    public int ComboCount { get; private set; } = 0;

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int, int> OnCombo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TotalScore = SaveManager.Instance.TotalScore;
        OnScoreChanged?.Invoke(TotalScore);
    }

    public void RegisterMatch()
    {
        ComboCount++;

        int points = ComboCount == 1 ? 1 : (int)Mathf.Pow(2, ComboCount - 1);
        TotalScore += points;

        SaveManager.Instance.SaveScore(TotalScore);
        OnScoreChanged?.Invoke(TotalScore);

        if (ComboCount >= 2)
            OnCombo?.Invoke(ComboCount, points);
    }

    public void ResetCombo()
    {
        ComboCount = 0;
    }

    public void ResetAll()
    {
        TotalScore = 0;
        ComboCount = 0;
        SaveManager.Instance.ResetAll();
        OnScoreChanged?.Invoke(TotalScore);
    }
}