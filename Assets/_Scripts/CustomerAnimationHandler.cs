using UnityEngine;

public class CustomerAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private Customer customer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        customer = GetComponent<Customer>();
    }
    private void OnEnable()
    {
        customer.OnStateChange += OnStateChangeHandle;
    }

    private void OnStateChangeHandle(Customer.CustomerState state)
    {
        switch (state)
        {
            case Customer.CustomerState.Searching:
                animator.Play("Walking");
                break;
            case Customer.CustomerState.EnjoyingLife:
                animator.Play("Interact");
                break;
            case Customer.CustomerState.Served:
                animator.Play("Walking");
                break;
        }
    }
}
