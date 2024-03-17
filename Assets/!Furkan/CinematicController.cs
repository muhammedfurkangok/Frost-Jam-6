using System;
using UnityEngine;

namespace _Furkan
{
    public class CinematicController : MonoBehaviour
    {
        private void Start()
        {
            GameSignals.Instance.onTextCompleted += OnTextCompeleted;
        }

        private void OnTextCompeleted()
        {
            Debug.Log("Text Completed");
        }

        private void OnDisable()
        {
            GameSignals.Instance.onTextCompleted -= OnTextCompeleted;
        }
    }
}