using System.Threading;
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
    public class Tween
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public Tween()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public Task Task => _taskCompletionSource.Task;
        public void Cancel() => _cancellationTokenSource.Cancel();
        public CancellationToken Token => _cancellationTokenSource.Token;
        internal void Complete() => _taskCompletionSource.TrySetResult(true);
    }
    public static async Task<Tween> MoveToAsync(this Transform transform, Vector3 targetPosition, float duration, Ease ease = Ease.Linear)
    {
        var tween = new Tween();
        await MoveToAsyncInternal(transform, targetPosition, duration, ease, tween);
        return tween;
    }
    private static async Task MoveToAsyncInternal(Transform transform, Vector3 targetPosition, float duration, Ease ease, Tween tween)
    {
        float timeElapsed = 0f;
        Vector3 startingPosition = transform.position;

        while (timeElapsed < duration)
        {
            if (tween.Token.IsCancellationRequested || transform == null)
                return;

            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;
            t = ApplyEase(ease, t);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            await Task.Yield();
        }

        if (!tween.Token.IsCancellationRequested)
        {
            transform.position = targetPosition;
            tween.Complete();
        }
    }
    public static async Task<Tween> MoveToSpeedBasedAsync(this Transform transform, Vector3 targetPosition, float speed, Ease ease = Ease.Linear)
    {
        var tween = new Tween();
        await MoveToSpeedBasedAsyncInternal(transform, targetPosition, speed, ease, tween);
        return tween;
    }
    private static async Task MoveToSpeedBasedAsyncInternal(Transform transform, Vector3 targetPosition, float speed, Ease ease, Tween tween)
    {
        Vector3 startingPosition = transform.position;
        float distance = Vector3.Distance(startingPosition, targetPosition);
        float duration = distance / speed;

        await MoveToAsyncInternal(transform, targetPosition, duration, ease, tween);
    }
    public static async Task<Tween> FillAmountAsync(this UnityEngine.UI.Image image, float progress, float duration)
    {
        var tween = new Tween();
        await FillAmountAsyncInternal(image, progress, duration, tween);
        return tween;
    }
    private static async Task FillAmountAsyncInternal(UnityEngine.UI.Image image, float progress, float duration, Tween tween)
    {
        float startFill = image.fillAmount;
        float elapsedTime = 0f;
        float target = startFill + progress;

        while (elapsedTime < duration)
        {
            if (tween.Token.IsCancellationRequested)
            {
                return;
            }

            float t = elapsedTime / duration;
            image.fillAmount = Mathf.Lerp(startFill, target, t);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        if (!tween.Token.IsCancellationRequested)
        {
            image.fillAmount = target;
            tween.Complete();
        }
    }
    public static async Task<Tween> DOFade(this UnityEngine.UI.Image image, float targetAlpha, float duration)
    {
        var tween = new Tween();
        await DOFadeInternal(image, targetAlpha, duration, tween);
        return tween;
    }
    private static async Task DOFadeInternal(UnityEngine.UI.Image image, float targetAlpha, float duration, Tween tween)
    {
        float elapsedTime = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            if (tween.Token.IsCancellationRequested)
            {
                return;
            }

            float t = elapsedTime / duration;
            image.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        if (!tween.Token.IsCancellationRequested)
        {
            image.color = targetColor;
            tween.Complete();
        }
    }
    public static async Task<Tween> PunchScale(this Transform transform, Vector2 punchAmount, float duration, int vibrato, float elasticity)
    {
        var tween = new Tween();
        await PunchScaleInternal(transform, punchAmount, duration, vibrato, elasticity, tween);
        return tween;
    }
    private static async Task PunchScaleInternal(Transform transform, Vector2 punchAmount, float duration, int vibrato, float elasticity, Tween tween)
    {
        Vector2 originalScale = transform.localScale;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            if (tween.Token.IsCancellationRequested)
            {
                return;
            }

            float strength = Mathf.Pow(1f - timeElapsed / duration, elasticity);
            Vector2 punch = punchAmount * Mathf.Sin(timeElapsed * vibrato * Mathf.PI * 2f / duration) * strength;

            transform.localScale = originalScale + punch;
            timeElapsed += Time.deltaTime;

            await Task.Yield();
        }

        if (!tween.Token.IsCancellationRequested)
        {
            transform.localScale = originalScale;
            tween.Complete();
        }
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
                return t;
        }
    }
}