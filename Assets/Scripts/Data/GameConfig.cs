using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameConfig", menuName = "MemoryGame/GameConfig")]
public class GameConfig : ScriptableObject
{
    public List<GameObject> cardPrefabs = new List<GameObject>();
    public List<LevelConfig> levels = new List<LevelConfig>();
    public float cardSpacing = 0.1f;
}

[System.Serializable]
public class LevelConfig
{
    public string levelName = "Level 1";
    public int columns = 4;
    public int rows = 4;

    public int TotalCards => columns * rows;
    public int PairsNeeded => TotalCards / 2;
    public bool IsValid() => TotalCards % 2 == 0;
}