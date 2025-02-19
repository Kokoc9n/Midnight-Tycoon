using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    private void Awake()
    {
        //GetComponentInParent<Canvas>().worldCamera = Camera.main;
    }
    void Start()
    {
        transform.LookAt(Camera.main.transform, Camera.main.transform.up);
    }
}
