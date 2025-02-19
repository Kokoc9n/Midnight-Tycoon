using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonClickHandler : MonoBehaviour
{
    private Button button;
    public virtual void Start()
    {
        //tween = transform.DOShakeScale(0.2f, 0.2f).Pause().SetAutoKill(false);
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        //button.onClick.AddListener(() => tween.Restart());
    }
    public abstract void OnButtonClicked();
}