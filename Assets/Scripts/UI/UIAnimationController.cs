using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    [Header("Fade Effects")]
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 0.5f;

    [Header("Scale Animation")]
    public bool animateScale = true;
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;
    public float scaleDuration = 0.8f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Text Animation")]
    public Text animatedText;
    public float typewriterSpeed = 0.05f;
    public bool typewriterEffect = false;

    [Header("Rotation Animation")]
    public bool rotateAnimation = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 90);

    private string originalText;
    private bool isAnimating = false;

    void Start()
    {
        if (animatedText != null)
        {
            originalText = animatedText.text;
            if (typewriterEffect)
                animatedText.text = "";
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (animateScale)
        {
            transform.localScale = startScale;
        }
    }

    void Update()
    {
        if (rotateAnimation && !isAnimating)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    public void FadeIn()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeInDuration));
    }

    public void FadeOut()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeOutDuration));
    }

    public void AnimateIn()
    {
        gameObject.SetActive(true);

        if (animateScale)
            StartCoroutine(ScaleAnimation(startScale, endScale));

        if (typewriterEffect && animatedText != null)
            StartCoroutine(TypewriterEffect());

        FadeIn();
    }

    public void AnimateOut()
    {
        if (animateScale)
            StartCoroutine(ScaleAnimation(endScale, startScale));

        FadeOut();

        StartCoroutine(DeactivateAfterDelay(fadeOutDuration));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }

        cg.alpha = end;
    }

    IEnumerator ScaleAnimation(Vector3 from, Vector3 to)
    {
        isAnimating = true;
        float elapsedTime = 0;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = scaleCurve.Evaluate(elapsedTime / scaleDuration);
            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.localScale = to;
        isAnimating = false;
    }

    IEnumerator TypewriterEffect()
    {
        if (animatedText == null || string.IsNullOrEmpty(originalText)) yield break;

        animatedText.text = "";

        for (int i = 0; i <= originalText.Length; i++)
        {
            animatedText.text = originalText.Substring(0, i);
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // Método para resetear el texto animado
    public void ResetText()
    {
        if (animatedText != null && !string.IsNullOrEmpty(originalText))
        {
            animatedText.text = typewriterEffect ? "" : originalText;
        }
    }

    // Método para cambiar el texto animado
    public void SetText(string newText)
    {
        originalText = newText;
        if (animatedText != null)
        {
            animatedText.text = typewriterEffect ? "" : newText;
        }
    }
}
