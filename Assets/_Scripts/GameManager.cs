using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] Service[] services;
    [SerializeField] Transform entrance;
    [SerializeField] Transform customerStartingPoint;
    private string savePath;

    public Action<int, Vector3?> OnProfit;

    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnDisable()
    {
        for (int i = 0; i < services.Length; i++)
        {
            services[i].OnServedCustomer -= OnServedCustomerHandle;
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < services.Length; i++)
        {
            services[i].OnServedCustomer += OnServedCustomerHandle;
        }
    }
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/playerData.json";
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Load();
    }
    public Vector3 GetEntrancePosition() => entrance.position;
    public Vector3 GetStartingPosition() => customerStartingPoint.position;
    public Service GetService() => services.FirstOrDefault();
    public Service[] GetServices() => services;
    private void OnServedCustomerHandle(int profit, Vector3 coords)
    {
        Player.Money += profit;
        OnProfit?.Invoke(Player.Money, coords);
    }
    public void Save()
    {
        PlayerData playerData = new PlayerData()
        {
            Money = Player.Money,
            Level = Player.Level
        };
        List<ServiceData> allServiceData = new List<ServiceData>();
        foreach (var service in services)
        {
            List<ModuleData> moduleDatas = new();
            foreach (var module in service.GetModules())
            {
                int index = 0;
                moduleDatas.Add(new ModuleData()
                {
                    Index = index,
                    Level = module.CurrentLevel
                });
            }
            ServiceData serviceData = new ServiceData()
            {
                Price = service.Price,
                ModulesData = moduleDatas.ToArray()
            };
            allServiceData.Add(serviceData);
        }

        string jsonData = JsonUtility.ToJson(new ServiceDataListWrapper() { Services = allServiceData });

        SaveDataWrapper wrapper = new SaveDataWrapper()
        {
            ServiceDataJson = jsonData,
            PlayerDataJson = JsonUtility.ToJson(playerData)
        };
        Debug.Log(wrapper.PlayerDataJson);
        Debug.Log(wrapper.ServiceDataJson);
        File.WriteAllText(savePath, JsonUtility.ToJson(wrapper));
    }
    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No saved services data found.");
            return;
        }
        string jsonData = File.ReadAllText(savePath);
        SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(jsonData);
        var playerData = JsonUtility.FromJson<PlayerData>(wrapper.PlayerDataJson);
        Player.Money = playerData.Money;
        Player.Level = playerData.Level;
        var servicesDataWrapper = JsonUtility.FromJson<ServiceDataListWrapper>(wrapper.ServiceDataJson);
        for (int i = 0; i < servicesDataWrapper.Services.Count; i++)
        {
            services[i].Init(servicesDataWrapper.Services[i]);
        }

    }
}