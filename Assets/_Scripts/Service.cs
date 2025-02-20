using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Raycaster;

public class Service : MonoBehaviour
{
    private const float SERVE_SPEED = 10;
    [SerializeField] int maxCustomerSpots;
    [SerializeField] Module[] modules;
    [SerializeField] ServiceSpot[] freeCustomerSpots;
    [SerializeField] ServiceView serviceView;
    private RaycastReceiver raycastReceiver;
    public int Price { get; private set; }
    private bool active = true;
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
    private void Awake()
    {
        raycastReceiver = new(transform);
        raycastReceiver.OnHitFirstClick += OnHitFirstClickHandle;
    }
    public void Init(ServiceData data)
    {
        Price = data.Price;
        for (int i = 0; i < modules.Length; i++)
        {
            modules[i].Init(data.ModulesData[i].Level);
        }
    }
    private void Start()
    {
        Available = true;
        Price = CalculatePrice(modules) + 1;
    }
    public void Toggle() =>
        active = !active;
    public Vector3 GetSpotPosition(int index) =>
        FreeCustomerSpots[index].Transform.position;
    public Module[] GetModules() => modules;
    public async void ServeCustomer(Customer customer)
    {
        var spot = FreeCustomerSpots.First(_ => _.Free == true);
        spot.Free = false;

        if (FreeCustomerSpots.All(_ => _.Free == false)) Available = false;

        customer.transform.LookAt(spot.Transform.position, transform.up);
        await customer.transform.MoveToSpeedBasedAsync(spot.Transform.position, 1f);
        customer.transform.rotation = spot.Transform.rotation;
        customer.OnSpotReachedHandle();

        OnServeInit?.Invoke(SERVE_SPEED, spot.Transform.position);
        await Task.Delay((int)SERVE_SPEED * 1000);
        var profit = customer.Money + Price;
        OnServedCustomer?.Invoke(profit, transform.position);
        spot.Free = Available = true;
        customer.OnServedHandle();
    }
    private void OnHitFirstClickHandle(Vector3 vector)
    {
        CanvasManager.StackPage(typeof(ServicePage));
        serviceView.Display(this);
        // Zoom in

    }
    private int CalculatePrice(Module[] modules)
    {
        int price = 0;
        foreach (var module in modules)
            price += Mathf.RoundToInt(module.Bonus);
        return price;
    }
}

