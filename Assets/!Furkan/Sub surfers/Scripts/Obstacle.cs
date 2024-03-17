using UnityEngine;

namespace _Furkan.Sub_surfers.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Obstacle linkedObstacle;
        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private MeshRenderer meshRenderer;

        public void DestroyObstacle()
        {
            linkedObstacle.DestroyObstacle();

            foreach (var particle in particles) particle.gameObject.SetActive(true);

            meshRenderer.enabled = false;
            Destroy(gameObject, 5f);
        }
    }
}