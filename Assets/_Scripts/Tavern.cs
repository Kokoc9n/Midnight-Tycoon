using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Tavern : MonoBehaviour
{
    public static Tavern Instance;
    [SerializeField] Service[] services;
    [SerializeField] Transform entrance;
    [SerializeField] Transform customerStartingPoint;

    public Action<int, Vector3?> OnProfit;

    private void OnApplicationQuit()
    {
        Player.Save();
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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private async void Start()
    {
        Player.Load();
        // Debug.

        CustomerFactory.Instance.CreateCustomer();
        await Task.Delay(2000);
        CustomerFactory.Instance.CreateCustomer();
        await Task.Delay(2000);
        CustomerFactory.Instance.CreateCustomer();
        await Task.Delay(2000);
        CustomerFactory.Instance.CreateCustomer();
        await Task.Delay(2000);
        CustomerFactory.Instance.CreateCustomer();
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
}