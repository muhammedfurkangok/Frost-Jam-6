using UnityEngine;

namespace _Furkan
{
    public class CoinController : MonoBehaviour
    {
        private int _score = 10;
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                PointManager.Instance.IncreaseScore(_score);
                Destroy(other.gameObject);
            }
        }
    }
}
