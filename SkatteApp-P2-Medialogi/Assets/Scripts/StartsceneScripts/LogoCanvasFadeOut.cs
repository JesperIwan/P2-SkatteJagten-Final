using UnityEngine;

public class LogoCanvasFadeOut : MonoBehaviour
{
    public CanvasGroup logoGroup;
    public float fadeDuration = 1f;
    public float waitBeforeFade = 2f; // seconds to wait before fading

    private void Start()
    {
        StartCoroutine(FadeOut());
        AudioManager.Instance.PlaySFX("IntroMusic");
    }

    private System.Collections.IEnumerator FadeOut()
    {
        // Wait before starting the fade
        yield return new WaitForSeconds(waitBeforeFade);

        float time = 0f;
        float startAlpha = logoGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            logoGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        logoGroup.alpha = 0f;
        logoGroup.interactable = false;
        logoGroup.blocksRaycasts = false;
    }
}

