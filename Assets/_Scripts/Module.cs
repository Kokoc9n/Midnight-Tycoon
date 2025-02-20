using UnityEngine;

public class Module : MonoBehaviour
{
    [SerializeField] ModuleUpgradeData[] upgradeData;
    [SerializeField] GameObject initialObject;

    public int CurrentLevel { get; private set; }
    public int NextLevelUpgradeCost { get; private set; }
    public float Bonus { get; private set; }
    public void Init(int level)
    {
        while(CurrentLevel != level)
        {
            Upgrade();
        }
    }
    private void Start()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        if(initialObject != null)
            initialObject.SetActive(true);
    }
    public void ApplyData(ModuleUpgradeData moduleUpgrade)
    {
        Bonus += moduleUpgrade.Bonus;
        if(moduleUpgrade.Add != null)
            moduleUpgrade.Add.SetActive(true);
        if (moduleUpgrade.SwapData.New == null) return;
        moduleUpgrade.SwapData.Old.SetActive(false);
        moduleUpgrade.SwapData.New.SetActive(true);
        NextLevelUpgradeCost = moduleUpgrade.UpgradeCost;
    }
    public void Upgrade()
    {
        if (CurrentLevel + 1 > upgradeData.Length)
        {
            Debug.LogWarning($"{gameObject.name} Exceded maximum level on upgrade");
            return;
        }
        if (Player.Money < NextLevelUpgradeCost) return;
        Player.Money -= NextLevelUpgradeCost;
        ApplyData(upgradeData[CurrentLevel++]);
    }
}

