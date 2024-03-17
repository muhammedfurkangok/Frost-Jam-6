using _Furkan.Sub_surfers.Scripts;
using UnityEngine;

public class SubwaySurfersManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform[] levels;

    [Header("Parameters")]
    [SerializeField] private float levelSpeed;

    [Header("Constants")]
    [SerializeField] private float level0Distance;
    [SerializeField] private float level1Distance;
    [SerializeField] private float level2Distance;

    [Header("Info")]
    public bool isGameActive;

    private void Start()
    {
        SubwaySurfersLevelMiddleChecker.OnMiddleCheckerPassed += OnMiddleCheckerPassed;
    }

    private void Update()
    {
        if (!isGameActive && playerController.isObstacleHit) return;

        foreach (var level in levels) level.Translate(Vector3.back * (levelSpeed * Time.deltaTime));
    }

    private void OnMiddleCheckerPassed()
    {
        levels[0].transform.position = levels[2].position + Vector3.forward * level2Distance;
        levels[1].transform.position = levels[0].position + Vector3.forward * level1Distance;
    }
}
