using UnityEngine;
using UnityEngine.UI;

public class WorldTimerView : MonoBehaviour
{
    [SerializeField] Image image;
    public void SetImageFillAmout(float amount)
    {
        image.fillAmount = amount;
    }
}
