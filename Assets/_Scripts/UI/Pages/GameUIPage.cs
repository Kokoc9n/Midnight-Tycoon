using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GameUIPage : Page
{
    [SerializeField] TMP_Text moneyCounter;
    [SerializeField] TMP_Text clockText;

    private ObjectPool<WorldTimerView> timerImagePool;
    private ObjectPool<Image> moneyImagePool;
    private int initialSize = 2;
    private int maxSize = 50;
    private IProgress<float> progressBar;

    private void OnDisable()
    {
        GameManager.Instance.OnProfit -= OnProfitHandle;
        TimeManager.Instance.OnGameTimeChanged -= OnGameTimeChangedHandle;
    }
    private void OnEnable()
    {
        GameManager.Instance.OnProfit += OnProfitHandle;
        TimeManager.Instance.OnGameTimeChanged += OnGameTimeChangedHandle;
        //OnProfitHandle(Player.Money, null);
        moneyCounter.text = $"{Player.Money}";
    }
    private void Awake()
    {
        var services = GameManager.Instance.GetServices();
        for (int i = 0; i < services.Length; i++)
        {
            services[i].OnServeInit += OnServeInitHandle;
        }
    }
    private void Start()
    {
        timerImagePool = new ObjectPool<WorldTimerView>(
          () => CreateTimerImage(),
          t => t.gameObject.SetActive(true),
          t => t.gameObject.SetActive(false),
          t => GameObject.Destroy(t.gameObject),
          defaultCapacity: initialSize,
          maxSize: maxSize
        );
        moneyImagePool = new ObjectPool<Image>(
          () => CreateMoneyImage(),
          t => t.gameObject.SetActive(true),
          t => t.gameObject.SetActive(false),
          t => GameObject.Destroy(t.gameObject),
          defaultCapacity: initialSize,
          maxSize: maxSize
        );
    }
    private WorldTimerView CreateTimerImage()
    {
        var obj = GameObject.Instantiate(Resources.Load("Prefabs/WorldTimer"), 
            CanvasManager.Instance.transform.parent) as GameObject; // Ew!
        return obj.transform.GetComponent<WorldTimerView>();
    }
    private Image CreateMoneyImage()
    {
        var obj = GameObject.Instantiate(Resources.Load("Prefabs/WorldMoney"),
            transform) as GameObject;
        Debug.Log(obj);
        return obj.GetComponentInChildren<Image>();
    }
    private async void OnProfitHandle(int money, Vector3? pos)
    {
        var moneyImage = moneyImagePool.Get();
        if (pos != null) /////
            moneyImage.transform.position = Camera.main.WorldToScreenPoint((Vector3)pos);
        moneyImage.transform.localScale = Vector3.one;
        moneyImage.color = Color.white;

        _ = moneyImage.transform.MoveToAsync(moneyCounter.transform.position, 0.5f);
        _ = moneyCounter.transform.PunchScale(new Vector2(0.2f, 0.2f), 1f, 3, 1f);
        await moneyImage.GetComponentInChildren<Image>().DOFade(0, 0.5f);

        moneyCounter.text = $"{money}";
        moneyCounter.transform.localScale = Vector3.one;
        moneyImagePool.Release(moneyImage);
    }
    private async Task OnServeInitHandle(float time, Vector3 pos)
    {
        var timerImage = timerImagePool.Get();
        timerImage.transform.position = pos + new Vector3(0, 5);
        await Timer(time);
        timerImagePool.Release(timerImage);
        await Task.CompletedTask;
        async Task Timer(float time)
        {
            float startTime = Time.time;
            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                elapsedTime = Time.time - startTime;
                float percent = Mathf.Clamp01(elapsedTime / time);
                timerImage.SetImageFillAmout(percent);
                await Task.Yield();
            }
            timerImage.SetImageFillAmout(1);
        }
    }
    private void OnGameTimeChangedHandle(float gameTime)
    {
        int hours = Mathf.FloorToInt(gameTime);
        int minutes = Mathf.FloorToInt((gameTime - hours) * 60);
        clockText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }
}
