using _Furkan;
using UnityEngine;
using UnityEngine.Video;

public class GameStarter : MonoBehaviour
{
    [Header("Video Players")]
    [SerializeField] private VideoPlayer familyGuyVideoPlayer;
    [SerializeField] private VideoPlayer subwaySurfersVideoPlayer;
    [SerializeField] private SubwaySurfersManager subwaySurfersManager;
    [SerializeField] private CarController carController;

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
        subwaySurfersManager.isGameActive = true;
        carController.isGameActive = false;

        familyGuyVideoPlayer.SetDirectAudioVolume(0,0.1f);
        subwaySurfersVideoPlayer.Play();
    }
}
