using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance { get; private set; }

    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Image congratsImage;
    [SerializeField] private Button nextLevelButton;

    [SerializeField] private GameObject completionParticlesPrefab;
    [SerializeField] private GameObject winPanelParticlesPrefab;

    [SerializeField] private float panelFadeDelay = 1f;
    [SerializeField] private float panelFadeDuration = 0.3f;
    [SerializeField] private float imageAppearDuration = 0.5f;
    [SerializeField] private float buttonAppearDuration = 0.4f;

    private GameObject spawnedCompletionParticles;
    private GameObject spawnedWinPanelParticles;

    private void Awake()
    {
        Instance = this;
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        congratsImage.transform.localScale = Vector3.zero;
        nextLevelButton.transform.localScale = Vector3.zero;
    }

    public void Show()
    {
        StartCoroutine(ShowRoutine());
    }

    private System.Collections.IEnumerator ShowRoutine()
    {
        if (completionParticlesPrefab != null)
        {
            spawnedCompletionParticles = Instantiate(completionParticlesPrefab, Vector3.zero, Quaternion.identity);
            spawnedCompletionParticles.transform.localScale = Vector3.one;
        }

        if (winPanelParticlesPrefab != null)
        {
            spawnedWinPanelParticles = Instantiate(winPanelParticlesPrefab, Vector3.zero, Quaternion.identity);
            spawnedWinPanelParticles.transform.localScale = Vector3.one;
        }

        SoundManager.Instance.PlayLevelComplete();

        yield return new WaitForSeconds(panelFadeDelay);

        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            panelCanvasGroup.DOFade(1f, panelFadeDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Append(
            congratsImage.transform.DOScale(1f, imageAppearDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Append(
            nextLevelButton.transform.DOScale(1f, buttonAppearDuration)
                .SetEase(Ease.OutBack)
        );
    }

    public void Hide()
    {
        StopAllCoroutines();

        panelCanvasGroup.DOKill();
        congratsImage.transform.DOKill();
        nextLevelButton.transform.DOKill();

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        congratsImage.transform.localScale = Vector3.zero;
        nextLevelButton.transform.localScale = Vector3.zero;

        if (spawnedCompletionParticles != null)
        {
            Destroy(spawnedCompletionParticles);
            spawnedCompletionParticles = null;
        }

        if (spawnedWinPanelParticles != null)
        {
            Destroy(spawnedWinPanelParticles);
            spawnedWinPanelParticles = null;
        }
    }
}