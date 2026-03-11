using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float punchDuration = 0.25f;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * -punchScale, punchDuration, 6, 0.5f)
            .OnComplete(() =>
            {
                LevelCompleteUI.Instance.Hide();
                ScoreManager.Instance.ResetCombo();
                gridSpawner.LoadLevel(SaveManager.Instance.CurrentLevel);
            });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}