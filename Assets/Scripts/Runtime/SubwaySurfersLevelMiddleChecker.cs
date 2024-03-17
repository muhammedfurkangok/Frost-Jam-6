using System;
using UnityEngine;

public class SubwaySurfersLevelMiddleChecker : MonoBehaviour
{
    public static event Action OnMiddleCheckerPassed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) OnMiddleCheckerPassed?.Invoke();
    }
}
