using UnityEngine.Events;

namespace _Furkan
{
    public class GameSignals: UnityEngine.MonoBehaviour
    {
        public static GameSignals Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public UnityAction onTextCompleted = delegate {  };
        public UnityAction onCameraComplete = delegate {  };
        public UnityAction onGameComplete = delegate {  };
    }
}