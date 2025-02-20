using TMPro;
using UnityEngine;

public class ServiceView : MonoBehaviour
{
    [SerializeField] TMP_Text serviceName;
    [SerializeField] TMP_Text profit;
    [SerializeField] Color toggleColor;
    [SerializeField] Transform content;

    [SerializeField] TMP_Text moduleName;
    [SerializeField] TMP_Text moduleLevel;
    [SerializeField] TMP_Text upgradeCost;
    [SerializeField] TMP_Text text;

    [SerializeField] UpgradeModuleButton upgradeButton;

    private Module selectedModule;
    private Service service;
    // TODO: fix
    // Incorrect data for secondary modules after upgrade.

    public void Display(Service service)
    {
        this.service = service;
        serviceName.text = service.transform.name;
        profit.text = service.Price.ToString();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        var modules = service.GetModules();
        selectedModule = modules[0];
        upgradeButton.Init(this, selectedModule, service);
        foreach (var module in modules)
        {
            var moduleGameobject = Instantiate(Resources.Load("Prefabs/ModuleButton"), content.transform) as GameObject;
            var moduleNameText = moduleGameobject.GetComponentInChildren<TMP_Text>();
            var moduleButton = moduleGameobject.GetComponent<ModuleButton>();

            moduleButton.Init(this, module);
            moduleNameText.text = module.name;
            ToggleModuleButton(module, moduleGameobject, moduleNameText);
        }

        moduleName.text = selectedModule.name;
        moduleLevel.text = selectedModule.CurrentLevel.ToString();
        upgradeCost.text = selectedModule.NextLevelUpgradeCost.ToString();
        text.text = $"Upgrade the {selectedModule.name} to increase cash earched.";
    }

    private void ToggleModuleButton(Module module, GameObject moduleButton, TMP_Text moduleNameText)
    {
        if (selectedModule == module)
        {
            moduleNameText.color = toggleColor;
            moduleButton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            moduleNameText.color = Color.white;
            moduleButton.GetComponent<UnityEngine.UI.Image>().color = toggleColor;
        }
    }

    public void SelectModule(Module module)
    {
        selectedModule = module;
        upgradeButton.Init(this, selectedModule, service);
        foreach (Transform buttonObject in content)
        {
            ToggleModuleButton(buttonObject.GetComponent<ModuleButton>().Module,
                               buttonObject.gameObject,
                               buttonObject.transform.GetComponentInChildren<TMP_Text>());
        }
        // Diplay data.
        moduleName.text = selectedModule.name;
        moduleLevel.text = selectedModule.CurrentLevel.ToString();
        upgradeCost.text = selectedModule.NextLevelUpgradeCost.ToString();
        text.text = $"Upgrade the {selectedModule.name} to increase cash earched.";
    }
}
