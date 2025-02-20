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
        Waiting = 3,
        WalkingToSpot = 4,
    }
    private void OnApplicationQuit()
    {
        tokenSource.Cancel();
    }
    private void Start()
    {
        transform.position = GameManager.Instance.GetStartingPosition();
        HandleSearchingState();
    }
    public void OnSpotReachedHandle()
    {
        currentState = CustomerState.EnjoyingLife;
        OnStateChange?.Invoke(currentState);
    }
    public async void OnServedHandle()
    {
        var ePosition = GameManager.Instance.GetEntrancePosition();
        var sPosition = GameManager.Instance.GetStartingPosition();
        currentState = CustomerState.Served;
        OnStateChange?.Invoke(currentState);
        await Task.Delay(200); // Hardcoded delay, EW!
        transform.LookAt(ePosition, transform.up);
        await transform.MoveToSpeedBasedAsync(ePosition, MOVE_SPEED);
        transform.LookAt(sPosition, transform.up);
        await transform.MoveToSpeedBasedAsync(sPosition, MOVE_SPEED);
        Destroy(gameObject);
    }
    private async void HandleSearchingState()
    {
        // Move to tavern door
        currentState = CustomerState.Searching;
        OnStateChange?.Invoke(currentState);

        var ePosition = GameManager.Instance.GetEntrancePosition();
        transform.LookAt(ePosition, transform.up);
        await transform.MoveToSpeedBasedAsync(ePosition, MOVE_SPEED);
        await WaitForService(GameManager.Instance.GetService());

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
                    currentState = CustomerState.WalkingToSpot;
                    OnStateChange?.Invoke(currentState);
                    service.ServeCustomer(this);
                    // OnStateChange
                    return;
                }
            }
            if (tokenSource.IsCancellationRequested) return;
            // OnPatienceRunOut
            currentState = CustomerState.Served;
            OnStateChange?.Invoke(currentState);
            Debug.Log("did not find a spot");
        }
    }
    
}

