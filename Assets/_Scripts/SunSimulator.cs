using UnityEngine;

public class SunSimulator : MonoBehaviour
{
    [SerializeField] Light directionalLight;
    [SerializeField] float intensityMultiplier = 1f;

    private void Update()
    {
        RotateSun(TimeManager.Instance.GameTime);
        AdjustLightProperties(TimeManager.Instance.GameTime);
    }

    private void RotateSun(float time)
    {
        float rotationAngle = Mathf.Lerp(-90f, 270f, Mathf.InverseLerp(6f, 18f, time));
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle, 170f, 0);
    }

    private void AdjustLightProperties(float time)
    {
        if (time >= 6f && time < 9f) // Morning
        {
            directionalLight.color = new Color(1f, 0.9f, 0.6f); // Warm orange
            directionalLight.intensity = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(6f, 9f, time)) * intensityMultiplier;
        }
        else if (time >= 9f && time < 18f) // Noon
        {
            directionalLight.color = new Color(1f, 1f, 0.9f); // Bright white/yellow
            directionalLight.intensity = Mathf.Lerp(1f, 1.5f, Mathf.InverseLerp(9f, 18f, time)) * intensityMultiplier;
        }
        else if (time >= 18f && time < 21f) // Evening
        {
            directionalLight.color = new Color(1f, 0.6f, 0.2f); // Soft orange/red
            directionalLight.intensity = Mathf.Lerp(1.5f, 0.5f, Mathf.InverseLerp(18f, 21f, time)) * intensityMultiplier;
        }
        else // Night
        {
            directionalLight.color = new Color(0.1f, 0.1f, 0.2f); // Dark blue (or night time color)
            directionalLight.intensity = 0f;
        }
    }
}
