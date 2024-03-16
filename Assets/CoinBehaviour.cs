
using System;
using DG.Tweening;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    private void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

   
}
