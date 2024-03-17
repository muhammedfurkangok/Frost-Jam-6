using System;
using _Furkan;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class JumpScareController : MonoBehaviour
{
    [Header("Game Complete Time")]
    [SerializeField] private float gameCompleteTime;

    [Header("References")]
    [SerializeField] private GameStarter gameStarter;
    [SerializeField] private AudioSource jumpScareAudioSource;
    [SerializeField] private float jumpScareDuration;
    [SerializeField] private Transform[] JumpScarePaths;
    [SerializeField] private Vector3[] JumpScarePathsVector3;

    [Header("Game Complete")]
    [SerializeField] private GameObject levelCompleteCanvas;

    private async void Start()
    {
        await UniTask.WaitForSeconds(gameCompleteTime);
        OnGameCompleted();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) OnGameCompleted();
    }

    private void OnGameCompleted()
    {
        gameStarter.CompleteGame();

        JumpScarePathsVector3 = new Vector3[JumpScarePaths.Length];
        for (int i = 0; i < JumpScarePaths.Length; i++)
        {
            JumpScarePathsVector3[i] = JumpScarePaths[i].position;
        }

        transform.DOPath(JumpScarePathsVector3, jumpScareDuration, PathType.CatmullRom).SetEase(Ease.Linear).OnWaypointChange(index =>
        {
            if (index == 3) jumpScareAudioSource.Play();
        }).OnComplete(() => levelCompleteCanvas.SetActive(true));
    }

    private void OnDisable()
    {
        GameSignals.Instance.onGameComplete -= OnGameCompleted;
    }
}
