using Raycaster;
using Unity.VisualScripting;
using UnityEngine;

public partial class Module : MonoBehaviour
{
    [SerializeField] ModuleUpgradeData[] upgradeData;
    [SerializeField] GameObject initialObject;
    private RaycastReceiver raycastReceiver;
    private int currentLevel;
    public float Bonus { get; private set; }
    private void Awake()
    {
        raycastReceiver = new(transform);
        raycastReceiver.OnHitFirstClick += OnHitFirstClickHandle;
    }
    private void Start()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        initialObject.SetActive(true);
        // Debug loading level
        //for (int i = 0; i < 2; i++)
        //{
        //    Upgrade();
        //}
    }
    public void ApplyData(ModuleUpgradeData moduleUpgrade)
    {
        Bonus += moduleUpgrade.Bonus;
        if(moduleUpgrade.Add != null)
            moduleUpgrade.Add.SetActive(true);
        if (moduleUpgrade.SwapData.New == null) return;
        moduleUpgrade.SwapData.Old.SetActive(false);
        moduleUpgrade.SwapData.New.SetActive(true);
    }
    private void OnHitFirstClickHandle(Vector3 vector)
    {
        // Open UI
        Upgrade();
    }
    public void Upgrade()
    {
        if (currentLevel + 1 > upgradeData.Length)
        {
            Debug.LogWarning($"{gameObject.name} Exceded maximum level on upgrade");
            return;
        }
        ApplyData(upgradeData[currentLevel++]);
    }
}

