using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] int sceneToLoad;
    [SerializeField] GameObject pauseParent;
    bool _paused;

    private void Update()
    {
        if(!_paused && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            _paused = true;
        }
        else if(_paused && Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
            _paused = false;
        }
    }
    private void Resume()
    {
       Time.timeScale = 1;
       pauseParent.gameObject.SetActive(false);
   }
    private void Pause()
    {
        Time.timeScale = 0;
        pauseParent.gameObject.SetActive(true);
    }
    public void Quit()
    {
         Application.Quit();
    }
    public void Restart()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
