// ScreenFader.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;

    [Header("References (auto-criadas se null)")]
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public Image overlayImage;

    [Header("Config")]
    public Color overlayColor = Color.black;
    public bool dontDestroyOnLoad = true;
    public int sortingOrder = 5000;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        EnsureSetup();
        SetAlpha(0f);
    }

    void EnsureSetup()
    {
        if (canvas == null)
        {
            canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            var scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (overlayImage == null)
        {
            var go = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(canvas.transform, false);
            overlayImage = go.GetComponent<Image>();
            overlayImage.color = overlayColor;
            overlayImage.raycastTarget = true; // bloqueia cliques
            var rt = overlayImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else
        {
            overlayImage.color = overlayColor;
            overlayImage.raycastTarget = true;
        }
    }

    public void SetAlpha(float a)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Clamp01(a);
    }

    public IEnumerator FadeTo(float target, float duration)
    {
        if (canvasGroup == null) yield break;

        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // independe do Time.timeScale
            float k = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
            canvasGroup.alpha = Mathf.Lerp(start, target, k);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    public IEnumerator FadeOut(float duration) => FadeTo(1f, duration);
    public IEnumerator FadeIn(float duration) => FadeTo(0f, duration);

    /// <summary>
    /// Faz fade-out, executa ação no escuro (onReachBlack), segura e depois fade-in.
    /// </summary>
    public IEnumerator FadeOutIn(float outDur, float hold, float inDur, System.Action onReachBlack = null)
    {
        yield return FadeOut(outDur);
        onReachBlack?.Invoke();
        if (hold > 0f) yield return new WaitForSecondsRealtime(hold);
        yield return FadeIn(inDur);
    }
}
