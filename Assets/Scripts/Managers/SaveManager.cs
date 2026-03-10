using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string KEY_LEVEL = "CurrentLevel";
    private const string KEY_SCORE = "TotalScore";

    public int CurrentLevel { get; private set; }
    public int TotalScore { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void SaveLevel(int level)
    {
        CurrentLevel = level;
        PlayerPrefs.SetInt(KEY_LEVEL, level);
        PlayerPrefs.Save();
    }

    public void SaveScore(int score)
    {
        TotalScore = score;
        PlayerPrefs.SetInt(KEY_SCORE, score);
        PlayerPrefs.Save();
    }

    public void SaveAll(int level, int score)
    {
        CurrentLevel = level;
        TotalScore = score;
        PlayerPrefs.SetInt(KEY_LEVEL, level);
        PlayerPrefs.SetInt(KEY_SCORE, score);
        PlayerPrefs.Save();
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_LEVEL);
        PlayerPrefs.DeleteKey(KEY_SCORE);
        PlayerPrefs.Save();
        Load();
    }

    private void Load()
    {
        CurrentLevel = PlayerPrefs.GetInt(KEY_LEVEL, 0);
        TotalScore = PlayerPrefs.GetInt(KEY_SCORE, 0);
    }
}