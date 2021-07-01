using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Fader : MonoBehaviour
{
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float minTimeScale = 0.5f;

    [SerializeField] private CharacterMovement playerMovement;


    public UnityEvent CameraSwitchEvent { get; private set; }

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        CameraSwitchEvent = new UnityEvent();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        //fadeOutDuration *= transitionTimeScale;
        //fadeInDuration *= transitionTimeScale;
    }

    public void CameraTransition()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutIn());
    }
    public IEnumerator FadeOutIn()
    {
        playerMovement.Pause(true);
        yield return FadeOut(fadeOutDuration);
        CameraSwitchEvent.Invoke();
        playerMovement.Pause(false, false);
        yield return FadeIn(fadeInDuration);
        Time.timeScale = 1f;
    }

    public IEnumerator FadeOut(float duration)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / duration;
            Time.timeScale = Mathf.Max(1f - canvasGroup.alpha, minTimeScale);
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / duration;
            Time.timeScale = Mathf.Max(1f - canvasGroup.alpha, minTimeScale);
            yield return null;
        }
    }
}