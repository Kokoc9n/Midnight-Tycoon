using UnityEngine;

public class CustomerFactory : MonoBehaviour
{
    public static CustomerFactory Instance { get; private set; }
    [SerializeField] Customer customerPrefab;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public Customer CreateCustomer()
    {
        return Instantiate(customerPrefab);
    }
}
