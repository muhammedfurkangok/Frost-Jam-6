using System.Collections;
using System.Collections.Generic;
using _Furkan;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class JumpScareController : MonoBehaviour
{
    [SerializeField] private Transform[] JumpScarePaths;
    [SerializeField] private Vector3[] JumpScarePathsVector3;
        
    private void Start()
    {

        OnGameComplete();
        GameSignals.Instance.onGameComplete += OnGameComplete;
        
        //TODO:YURUME SESI VE JUMP SACRE SESI EKLENECEK
    }

    private async void OnGameComplete()
    {
        JumpScarePathsVector3 = new Vector3[JumpScarePaths.Length];
        for (int i = 0; i < JumpScarePaths.Length; i++)
        {
            JumpScarePathsVector3[i] = JumpScarePaths[i].position;
        }
            
        await transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

        transform.DOPath(JumpScarePathsVector3, 5f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
        });
    }

    private void OnDisable()
    {
        GameSignals.Instance.onGameComplete -= OnGameComplete;
    }
}

