using System;
using _Furkan;
using UnityEngine;
using UnityEngine.Video;

public class GameStarter : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private VideoPlayer familyGuyVideoPlayer;

    [SerializeField] private VideoPlayer subwaySurfersVideoPlayer;
    [SerializeField] private SubwaySurfersManager subwaySurfersManager;
    [SerializeField] private CarController carController;
    [SerializeField] private GameObject subwaySurfersPlane;
    [SerializeField] private GameObject carPlane;

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
        ChangeGame(true);

        familyGuyVideoPlayer.SetDirectAudioVolume(0, 0.1f);
        subwaySurfersVideoPlayer.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) ChangeGame(true);
        if (Input.GetKeyDown(KeyCode.K)) ChangeGame(false);
    }

    private void ChangeGame(bool willSubwayBeActive)
    {
        subwaySurfersManager.isGameActive = willSubwayBeActive;
        carController.isGameActive = !willSubwayBeActive;

        subwaySurfersPlane.SetActive(willSubwayBeActive);
        carPlane.SetActive(!willSubwayBeActive);
    }
}
