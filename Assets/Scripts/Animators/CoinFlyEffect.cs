using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class CoinFlyEffect : MonoBehaviour
{
    public static CoinFlyEffect Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform coinsTargetUI;
    [SerializeField] private Camera gameCamera;

    [Header("Spritesheet")]
    [SerializeField] private string spritesheetPath = "Spritesheet/Coin spritesheet";
    [SerializeField] private int columns = 11;
    [SerializeField] private int rows = 1;
    [SerializeField] private float frameRate = 24f;

    [Header("Animation")]
    [SerializeField] private float scatterRadius = 100f;
    [SerializeField] private float scatterDuration = 0.35f;
    [SerializeField] private float holdDuration = 0.15f;
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float coinSpawnDelay = 0.08f;
    [SerializeField] private float coinSize = 64f;

    private Sprite[] _sprites;
    private readonly List<GameObject> _pool = new List<GameObject>();
    private RectTransform _canvasRect;

    private void Awake()
    {
        Instance = this;
        if (gameCamera == null) gameCamera = Camera.main;
        _canvasRect = canvas.GetComponent<RectTransform>();
        LoadSprites();
    }

    private void LoadSprites()
    {
        Texture2D sheet = Resources.Load<Texture2D>(spritesheetPath);
        if (sheet == null)
        {
            Debug.LogError($"CoinFlyEffect: spritesheet not found at Resources/{spritesheetPath}");
            return;
        }

        _sprites = new Sprite[columns * rows];
        float w = (float)sheet.width / columns;
        float h = (float)sheet.height / rows;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int i = r * columns + c;
                Rect rect = new Rect(c * w, (rows - 1 - r) * h, w, h);
                _sprites[i] = Sprite.Create(sheet, rect, new Vector2(0.5f, 0.5f));
            }
        }
    }

    public void SpawnCoins(Vector3 worldSpawnPos, int count)
    {
        if (_sprites == null || _sprites.Length == 0)
        {
            Debug.LogWarning("CoinFlyEffect: sprites not loaded");
            return;
        }

        count = Mathf.Clamp(count, 1, 8);
        Vector2 sourceScreen = gameCamera.WorldToScreenPoint(worldSpawnPos);
        StartCoroutine(SpawnRoutine(sourceScreen, count));
    }

    private IEnumerator SpawnRoutine(Vector2 sourceScreen, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingleCoin(sourceScreen);
            yield return new WaitForSeconds(coinSpawnDelay);
        }
    }

    private void SpawnSingleCoin(Vector2 sourceScreen)
    {
        GameObject go = GetFromPool();
        go.transform.SetParent(canvas.transform, false);
        go.SetActive(true);

        RectTransform rt = go.GetComponent<RectTransform>();
        Image img = go.GetComponent<Image>();
        CoinSpritesheetAnimator anim = go.GetComponent<CoinSpritesheetAnimator>();

        rt.sizeDelta = new Vector2(coinSize, coinSize);
        rt.localScale = Vector3.zero;
        img.sprite = _sprites[0];

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : gameCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, sourceScreen, cam, out Vector2 localSource);
        rt.anchoredPosition = localSource;

        anim.Initialize(img, _sprites, frameRate);
        anim.StartAnimation();

        Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(cam, coinsTargetUI.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, targetScreen, cam, out Vector2 localTarget);

        Vector2 scatter = Random.insideUnitCircle * scatterRadius;
        Vector2 scatterPos = localSource + scatter;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        seq.Join(rt.DOAnchorPos(scatterPos, scatterDuration).SetEase(Ease.OutQuad));
        seq.AppendInterval(holdDuration);
        seq.Append(rt.DOAnchorPos(localTarget, flyDuration).SetEase(Ease.InBack));
        seq.Join(rt.DOScale(0.5f, flyDuration * 0.8f).SetEase(Ease.InQuad));
        seq.AppendCallback(() =>
        {
            rt.DOPunchScale(Vector3.one * 0.2f, 0.15f, 4, 0.3f);
            coinsTargetUI.DOKill();
            coinsTargetUI.localScale = Vector3.one;
            coinsTargetUI.DOPunchScale(Vector3.one * 0.25f, 0.3f, 6, 0.5f);
        });
        seq.AppendInterval(0.3f);
        seq.OnComplete(() =>
        {
            anim.StopAnimation();
            go.SetActive(false);
            go.transform.SetParent(transform, false);
        });
    }

    private GameObject GetFromPool()
    {
        foreach (var obj in _pool)
        {
            if (obj != null && !obj.activeSelf)
                return obj;
        }

        GameObject go = new GameObject("CoinParticle");
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>();
        go.AddComponent<CoinSpritesheetAnimator>();
        _pool.Add(go);
        return go;
    }
}