using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    private const float REAL_TIME_DURATION = 600f / 24f;

    public static TimeManager Instance;
    public Action<float> OnGameTimeChanged;

    public Action OnMidnight;
    public Action OnDawn;
    public Action OnMidday;
    public Action OnDusk;

    private int lastHour = -1;
    private int lastMinute = -1;
    public float GameTime { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Update()
    {
        GameTime += Time.deltaTime / REAL_TIME_DURATION;

        if (GameTime >= 24f)
            GameTime -= 24f;
        int hours = Mathf.FloorToInt(GameTime);
        int minutes = Mathf.FloorToInt((GameTime - hours) * 60);

        if (hours != lastHour || minutes != lastMinute)
        {
            OnGameTimeChanged?.Invoke(GameTime);
            CheckForSpecificTimes(hours);
            lastHour = hours;
            lastMinute = minutes;
        }
    }
    private void CheckForSpecificTimes(int hours)
    {
        switch (hours)
        {
            case 0:
                OnMidnight?.Invoke();
                break;
            case 6:
                OnDawn?.Invoke();
                break;
            case 12:
                OnMidday?.Invoke();
                break;
            case 18:
                OnDusk?.Invoke();
                break;
        }
    }
}
