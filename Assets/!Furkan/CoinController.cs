using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Furkan
{
    public class CoinController : MonoBehaviour
    {
        private int _score = 10;
        private async void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                PointManager.Instance.IncreaseScore(_score);
                other.gameObject.SetActive(false);
                await UniTask.WaitForSeconds(5);
                other.gameObject.SetActive(true);
            }
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (!CompareTag("Car")) return;

            if (other.gameObject.CompareTag("Coin"))
            {
                //PointManager.Instance.IncreaseScore(_score);
                other.gameObject.SetActive(false);
                await UniTask.WaitForSeconds(5);
                other.gameObject.SetActive(true);
            }
        }
    }
}
