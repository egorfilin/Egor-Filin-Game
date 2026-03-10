using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private Transform cardZone;
    [SerializeField] private GameConfig config;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float screenPaddingX = 0.9f;
    [SerializeField] private float screenPaddingY = 0.9f;

    private List<Card> spawnedCards = new List<Card>();
    private List<Vector3> debugPositions = new List<Vector3>();
    private float debugCardSize;

    private void Start()
    {
        FitCardZoneToScreen();
        SpawnCards(config.levels[0]);
    }

    private void FitCardZoneToScreen()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        float screenH = 2f * gameCamera.orthographicSize;
        float screenW = screenH * gameCamera.aspect;

        cardZone.localScale = new Vector3(
            screenW * screenPaddingX,
            screenH * screenPaddingY,
            cardZone.localScale.z
        );
    }

    public List<Card> SpawnCards(LevelConfig levelConfig)
    {
        if (levelConfig.TotalCards % 2 != 0)
            return spawnedCards;

        ClearCards();
        debugPositions.Clear();

        int cols = levelConfig.columns;
        int rows = levelConfig.rows;
        float spacing = config.cardSpacing;

        float zoneW = GetWorldScaleX(cardZone);
        float zoneH = GetWorldScaleY(cardZone);

        float cardSize = Mathf.Min(
            (zoneW - spacing * (cols - 1)) / cols,
            (zoneH - spacing * (rows - 1)) / rows
        );

        if (cardSize <= 0)
            return spawnedCards;

        debugCardSize = cardSize;

        float stepX = cardSize + spacing;
        float stepY = cardSize + spacing;

        float gridW = cols * cardSize + (cols - 1) * spacing;
        float gridH = rows * cardSize + (rows - 1) * spacing;

        Vector3 center = cardZone.position;
        float startX = center.x - gridW / 2f + cardSize / 2f;
        float startY = center.y + gridH / 2f - cardSize / 2f;

        List<(GameObject prefab, int pairId)> deck = BuildShuffledDeck(levelConfig);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;

                Vector3 pos = new Vector3(
                    startX + col * stepX,
                    startY - row * stepY,
                    center.z
                );

                debugPositions.Add(pos);

                GameObject go = Instantiate(deck[index].prefab, pos, Quaternion.identity);
                go.transform.localScale = new Vector3(cardSize, cardSize, 1f);

                Card card = go.GetComponent<Card>();
                card.Init(deck[index].pairId);
                spawnedCards.Add(card);
            }
        }

        return spawnedCards;
    }

    private float GetWorldScaleX(Transform t)
    {
        float s = 1f;
        while (t != null) { s *= t.localScale.x; t = t.parent; }
        return Mathf.Abs(s);
    }

    private float GetWorldScaleY(Transform t)
    {
        float s = 1f;
        while (t != null) { s *= t.localScale.y; t = t.parent; }
        return Mathf.Abs(s);
    }

    private List<(GameObject prefab, int pairId)> BuildShuffledDeck(LevelConfig levelConfig)
    {
        int pairsNeeded = levelConfig.PairsNeeded;
        List<GameObject> prefabs = config.cardPrefabs;
        List<(GameObject, int)> deck = new List<(GameObject, int)>();
        for (int i = 0; i < pairsNeeded; i++)
        {
            int idx = i % prefabs.Count;
            deck.Add((prefabs[idx], idx));
            deck.Add((prefabs[idx], idx));
        }
        Shuffle(deck);
        return deck;
    }

    private void ClearCards()
    {
        foreach (var card in spawnedCards)
            if (card != null) Destroy(card.gameObject);
        spawnedCards.Clear();
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void OnDrawGizmos()
    {
        if (cardZone == null) return;

        Gizmos.color = Color.green;
        float w = GetWorldScaleX(cardZone);
        float h = GetWorldScaleY(cardZone);
        Gizmos.DrawWireCube(cardZone.position, new Vector3(w, h, 0.1f));

        if (debugPositions.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var pos in debugPositions)
                Gizmos.DrawWireCube(pos, new Vector3(debugCardSize, debugCardSize, 0.1f));
        }
    }
}