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
    [SerializeField] private GameObject slider;
        
    private void Start()
    {
        GameSignals.Instance.onTextCompleted += OnTextCompeleted;
    }

    private async void OnTextCompeleted()
    {
        JumpScarePathsVector3 = new Vector3[JumpScarePaths.Length];
        for (int i = 0; i < JumpScarePaths.Length; i++)
        {
            JumpScarePathsVector3[i] = JumpScarePaths[i].position;
        }
            
        await transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            
        transform.DOPath (JumpScarePathsVector3, 5f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameSignals.Instance.onCameraComplete?.Invoke();
            slider.SetActive(true);
        }).OnWaypointChange((index) =>
        {
            if (index == 1)
            {
                transform.DORotate(new Vector3(0, 180, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            }
        });
    }

    private void OnDisable()
    {
        GameSignals.Instance.onTextCompleted -= OnTextCompeleted;
    }
}
}
}
