using System.Collections;
using _Furkan;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TypeWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thisText;
    [SerializeField] private CanvasGroup _canvasGroup;
    public float delay;
    public float soundDelay;
    public AudioClip TypeSound;
    [Multiline] public string yazi;
    
  
    AudioSource audSrc;

    private void Start()
    {
        audSrc = GetComponent<AudioSource>();

        StartCoroutine(TypeWrite());
        TypeSound1();
    }

    IEnumerator TypeWrite()
    {
       foreach(char i in yazi)
        {
            WriteCharWithSound(i);
            if(i.ToString() == ".") { yield return new WaitForSeconds(0.4f); } 
            yield return new WaitForSeconds(delay);         
        }
         
        FadeOutCanvas();
        
        
    }

    private async void TypeSound1()
    {
        foreach (char i in yazi)
        {
            audSrc.PlayOneShot(TypeSound);
            await UniTask.WaitForSeconds(soundDelay);
        }
    }

    void WriteCharWithSound(char i)
    {
        thisText.text += i.ToString();
    }

    private void FadeOutCanvas()
    {
        DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0, 1).SetEase(Ease.InSine).onComplete += () => GameSignals.Instance.onTextCompleted.Invoke();
    }
}