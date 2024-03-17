using System.Collections;
using _Furkan;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TypeWriter : MonoBehaviour
{
    [SerializeField] private Transform[] CreaturePaths;
    [SerializeField] private Vector3[] CreatureVector3;

        
    private void Start()
    {
        GameSignals.Instance.onTextCompleted += OnTextCompeleted;
            
    }

    private async void OnTextCompeleted()
    {
        CreatureVector3 = new Vector3[CreaturePaths.Length];
        for (int i = 0; i < CreaturePaths.Length; i++)
        {
            CreatureVector3[i] = CreaturePaths[i].position;
        }
            
        await transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

        transform.DOPath(CreatureVector3, 5f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameSignals.Instance.onCameraComplete?.Invoke();
            
        });
    }

    private void OnDisable()
    {
        GameSignals.Instance.onTextCompleted -= OnTextCompeleted;
    }

}
