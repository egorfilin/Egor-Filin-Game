using UnityEngine;

public class Card : MonoBehaviour
{
    public int PairId { get; private set; }

    public void Init(int pairId)
    {
        PairId = pairId;
    }
}