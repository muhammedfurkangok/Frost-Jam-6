using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;
    private int _score;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
    }
    
    public void IncreaseScore(int increaseAmount)
    {
        _score += increaseAmount;
    }
}
