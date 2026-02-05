using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuyoClearEffect : MonoBehaviour
{
    [SerializeField] private Image uiImage;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float scaleMultiplier = 1.6f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private Coroutine playingRoutine;

    private void Awake()
    {
        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void PlayUI(RectTransform source, Vector2 size)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = source.anchoredPosition;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = size;
        }

        PlayRoutine(true);
    }

    public void PlayWorld(Vector3 position)
    {
        transform.position = position;
        transform.localScale = Vector3.one;
        PlayRoutine(false);
    }

    private void PlayRoutine(bool isUI)
    {
        if (playingRoutine != null)
        {
            StopCoroutine(playingRoutine);
        }

        playingRoutine = StartCoroutine(EffectRoutine(isUI));
    }

    private IEnumerator EffectRoutine(bool isUI)
    {
        float elapsed = 0f;
        float effectDuration = Mathf.Max(0.01f, duration);
        Vector3 baseScale = isUI && rectTransform != null ? rectTransform.localScale : transform.localScale;

        while (elapsed < effectDuration)
        {
            float t = elapsed / effectDuration;
            float scaleT = scaleCurve.Evaluate(t);
            float fadeT = fadeCurve.Evaluate(t);
            Vector3 scale = Vector3.Lerp(baseScale, baseScale * scaleMultiplier, scaleT);
            float rotation = rotationSpeed * Time.deltaTime;

            if (isUI && rectTransform != null)
            {
                rectTransform.localScale = scale;
                rectTransform.Rotate(0f, 0f, rotation);
            }
            else
            {
                transform.localScale = scale;
                transform.Rotate(0f, 0f, rotation);
            }

            SetAlpha(fadeT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
        Destroy(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        if (uiImage != null)
        {
            Color color = uiImage.color;
            color.a = alpha;
            uiImage.color = color;
        }

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
