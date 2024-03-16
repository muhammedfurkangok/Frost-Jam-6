using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum GameFlags
{
    ClutterRoom,
    Lights,
    Fireplace
}

public class GameFlagManager : MonoBehaviour
{
    [Header("Game Parameters")]
    [SerializeField] private float[] flagTimes;

    [Header("Game Info")]
    [SerializeField] private GameFlags currentFlag;

    private void Start()
    {
        StartGameLoop();
    }

    private async void StartGameLoop()
    {
        await UniTask.WaitForSeconds(flagTimes[0]);
        currentFlag = GameFlags.ClutterRoom;
        ExecuteClutterRoomFlag();

        await UniTask.WaitForSeconds(flagTimes[1]);
        currentFlag = GameFlags.Lights;
        ExecuteLightsFlag();

        await UniTask.WaitForSeconds(flagTimes[2]);
        currentFlag = GameFlags.Fireplace;
        ExecuteFireplaceFlag();
    }

    private void ExecuteClutterRoomFlag()
    {

    }

    private void ExecuteLightsFlag()
    {

    }

    private void ExecuteFireplaceFlag()
    {

    }
}
