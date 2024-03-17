using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Furkan
{
    public class CinematicController : MonoBehaviour
    {
        [SerializeField] private Transform[] CameraPaths;
        [SerializeField] private Vector3[] CameraPathsVector3;
        [SerializeField] private GameObject slider;
        
        private void Start()
        {
            GameSignals.Instance.onTextCompleted += OnTextCompeleted;
        }

        private async void OnTextCompeleted()
        {
            CameraPathsVector3 = new Vector3[CameraPaths.Length];
            for (int i = 0; i < CameraPaths.Length; i++)
            {
                CameraPathsVector3[i] = CameraPaths[i].position;
            }
            
            await transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            
            transform.DOPath (CameraPathsVector3, 5f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
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