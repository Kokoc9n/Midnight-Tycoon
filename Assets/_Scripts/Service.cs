using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Service : MonoBehaviour
{
    [SerializeField] int maxCustomerSpots;
    [SerializeField] Module[] modules;
    [SerializeField] ServiceSpot[] freeCustomerSpots;
    private int price;
    private bool active;
    [System.Serializable]
    public class ServiceSpot
    {
        public Transform Transform;
        public bool Free;
    }
    public Action<int, Vector3> OnServedCustomer;
    public delegate Task EventHandler(float time, Vector3 pos);
    public EventHandler OnServeInit;
    public ServiceSpot[] FreeCustomerSpots { get => freeCustomerSpots; set => freeCustomerSpots = value; }
    public bool Available { get; private set; }
    private void Start()
    {
        // Debug
        Toggle();
        Available = true;
        price = CalculatePrice(modules) + 1;
    }
    public void Toggle() =>
        active = !active;
    public Vector3 GetSpotPosition(int index) =>
        FreeCustomerSpots[index].Transform.position;
    public async void ServeCustomer(Customer customer)
    {
        var spot = FreeCustomerSpots.First(_ => _.Free == true);
        spot.Free = false;
        Debug.Log(spot.Transform.name + " occupied " + spot.Free);
        if (FreeCustomerSpots.All(_ => _.Free == false)) Available = false;

        await customer.transform.MoveToSpeedBasedAsync(spot.Transform.position, 1f);

        OnServeInit?.Invoke(5, spot.Transform.position);
        await Task.Delay(5000);
        var profit = customer.Money + price;
        OnServedCustomer?.Invoke(profit, transform.position);
        spot.Free = Available = true;
        customer.OnServedHandle();
    }
    public void Save()
    {

    }
    public void Load()
    {

    }
    private int CalculatePrice(Module[] modules)
    {
        int price = 0;
        foreach (var module in modules)
            price += Mathf.RoundToInt(module.Bonus);
        return price;
    }
}

