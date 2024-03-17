using System.Collections;
using _Furkan;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TypeWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thisText;
    [SerializeField] private CanvasGroup _canvasGroup;
    public float delay = 0.1f;
    public AudioClip TypeSound;
    [Multiline] public string yazi;
  
    AudioSource audSrc;

    private void Start()
    {
        audSrc = GetComponent<AudioSource>();

        StartCoroutine(TypeWrite());
    }

    IEnumerator TypeWrite()
    {
        foreach(char i in yazi)
        {
            thisText.text += i.ToString();

            audSrc.pitch = Random.Range(0.8f, 1.2f);
            audSrc.PlayOneShot(TypeSound);

            if(i.ToString() == ".") { yield return new WaitForSeconds(1); }
            else { yield return new WaitForSeconds(delay); }          
        }

        FadeOutCanvas();
    }

    private void FadeOutCanvas()
    {
        DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0, 1).SetEase(Ease.InSine).onComplete += () => GameSignals.Instance.onTextCompleted.Invoke();
    }
}