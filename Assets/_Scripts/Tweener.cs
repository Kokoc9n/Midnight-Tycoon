using System.Threading.Tasks;
using UnityEngine;

internal static class Tweener
{
    public enum Ease
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad
    }
    public static async Task MoveToAsync(this Transform transform, Vector3 targetPosition, float duration, Ease ease = Ease.Linear)
    {
        float timeElapsed = 0f;
        Vector3 startingPosition = transform.position;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;
            t = ApplyEase(ease, t);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            await Task.Yield();
        }

        transform.position = targetPosition;
    }
    public static async Task MoveToSpeedBasedAsync(this Transform transform, Vector3 targetPosition, float speed, Ease ease = Ease.Linear)
    {
        Vector3 startingPosition = transform.position;
        float distance = Vector3.Distance(startingPosition, targetPosition);
        float duration = distance / speed;

        await MoveToAsync(transform, targetPosition, duration, ease);
    }
    public static async Task FillAmountAsync(this UnityEngine.UI.Image image, float progress, float duration)
    {
        float startFill = image.fillAmount;
        float elapsedTime = 0f;
        float target = startFill + progress;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            image.fillAmount = Mathf.Lerp(startFill, target, t);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
        image.fillAmount = target;
    }
    public static async Task DOFade(this UnityEngine.UI.Image image, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;

        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            image.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
        image.color = targetColor;
    }
    public static async Task PunchScale(this Transform transform, Vector2 punchAmount, float duration, int vibrato, float elasticity)
    {
        Vector2 originalScale = transform.localScale;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float strength = Mathf.Pow(1f - timeElapsed / duration, elasticity);
            Vector2 punch = punchAmount * Mathf.Sin(timeElapsed * vibrato * Mathf.PI * 2f / duration) * strength;

            transform.localScale = originalScale + punch;
            timeElapsed += Time.deltaTime;

            await Task.Yield();
        }
        transform.localScale = originalScale;
    }
    private static float ApplyEase(Ease ease, float t)
    {
        switch (ease)
        {
            case Ease.EaseInQuad:
                return t * t;
            case Ease.EaseOutQuad:
                return 1 - (1 - t) * (1 - t);
            case Ease.EaseInOutQuad:
                return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
            case Ease.Linear:
            default:
                return t; // No easing, just linear interpolation
        }
    }
}