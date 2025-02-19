using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private const float MOVE_SPEED = 2f;
    private int money = 1;
    public CustomerState currentState;
    //private int tasteType;
    private float maxPatience = 20;
    private CancellationTokenSource tokenSource = new();
    public Action<CustomerState> OnStateChange;
    public int Money { get => money; private set => money = value; }

    public enum CustomerState
    {
        Searching = 0,
        EnjoyingLife = 1,
        Served = 2,
        Waiting = 3
    }
    private void OnApplicationQuit()
    {
        tokenSource.Cancel();
    }
    private void Start()
    {
        transform.position = Tavern.Instance.GetStartingPosition();
        HandleSearchingState();
    }
    public async void OnServedHandle()
    {
        var ePosition = Tavern.Instance.GetEntrancePosition();
        await transform.MoveToSpeedBasedAsync(ePosition, MOVE_SPEED);
        await transform.MoveToSpeedBasedAsync(Tavern.Instance.GetStartingPosition(), MOVE_SPEED);
        Destroy(gameObject);
    }
    private async void HandleSearchingState()
    {
        // Move to tavern door
        currentState = CustomerState.Searching;
        OnStateChange?.Invoke(currentState);

        var ePosition = Tavern.Instance.GetEntrancePosition();
        transform.LookAt(ePosition, transform.up);
        await transform.MoveToSpeedBasedAsync(ePosition, MOVE_SPEED);
        await WaitForService(Tavern.Instance.GetService());

        async Task WaitForService(Service service)
        {
            float time = Time.time;
            while (Time.time - time <= maxPatience)
            {
                if (tokenSource.IsCancellationRequested) break;
                await Task.Yield();
                if (service.Available == true)
                {
                    Debug.Log("found a spot");
                    currentState = CustomerState.EnjoyingLife;
                    OnStateChange?.Invoke(currentState);
                    service.ServeCustomer(this);
                    // OnStateChange
                    return;
                }
            }
            // OnPatienceRunOut
            currentState = CustomerState.Served;
            OnStateChange?.Invoke(currentState);
            Debug.Log("did not find a spot");
        }
    }
    
}

