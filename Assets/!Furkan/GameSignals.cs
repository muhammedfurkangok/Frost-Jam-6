using UnityEngine.Events;

namespace _Furkan
{
    public class GameSignals: UnityEngine.MonoBehaviour
    {
        public static GameSignals Instance;
        
        #region Singleton

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion
        public UnityAction onTextCompleted = delegate {  };
        public UnityAction onCameraComplete = delegate {  };
    }
}