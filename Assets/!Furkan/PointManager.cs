using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    
    public static PointManager Instance;
    private int _score;
    
    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        #endregion
    }
    
    public void IncreaseScore(int increaseAmount)
    {
        _score += increaseAmount;
    }
}
