using UnityEngine;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private GridSpawner gridSpawner;

    public void OnClick()
    {
        LevelCompleteUI.Instance.Hide();
        ScoreManager.Instance.ResetCombo();
        gridSpawner.LoadLevel(SaveManager.Instance.CurrentLevel);
    }
}