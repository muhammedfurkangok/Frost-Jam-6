using System;
using _Furkan;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameStarter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private VideoPlayer familyGuyVideoPlayer;
    [SerializeField] private VideoPlayer subwaySurfersVideoPlayer;
    [SerializeField] private SubwaySurfersManager subwaySurfersManager;
    [SerializeField] private CarController carController;
    [SerializeField] private GameObject subwaySurfersPlane;
    [SerializeField] private GameObject carPlane;

    [Header("Parameters")]
    [SerializeField] private float gameChangeTime;

    [Header("Game Complete")]
    [SerializeField] private GameObject videoPlane;
    [SerializeField] private Slider slider;

    private bool isGameCompleted;
    public bool isCurrentGameSubway;

    private void Start()
    {
        GameSignals.Instance.onCameraComplete += OnCameraComplete;
    }

    private void OnDisable()
    {
        GameSignals.Instance.onCameraComplete -= OnCameraComplete;
    }

    private void OnCameraComplete()
    {
        ChangeGame();
        StartGameChangeLoop();

        familyGuyVideoPlayer.SetDirectAudioVolume(0, 0.1f);
        subwaySurfersVideoPlayer.Play();
    }

    private async void StartGameChangeLoop()
    {
        while (!isGameCompleted)
        {
            await UniTask.WaitForSeconds(gameChangeTime);
            ChangeGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) ChangeGame();
    }

    private void ChangeGame()
    {
        isCurrentGameSubway = !isCurrentGameSubway;

        subwaySurfersManager.isGameActive = isCurrentGameSubway;
        carController.isGameActive = !isCurrentGameSubway;

        subwaySurfersPlane.SetActive(isCurrentGameSubway);
        carPlane.SetActive(!isCurrentGameSubway);
    }

    public void CompleteGame()
    {
        isGameCompleted = true;

        subwaySurfersManager.isGameActive = false;
        carController.isGameActive = false;

        subwaySurfersPlane.SetActive(false);
        carPlane.SetActive(false);

        slider.gameObject.SetActive(false);
        videoPlane.SetActive(false);
    }
}
