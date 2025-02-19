using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static ProgressBar Instance { get; private set; }
    public Image Image { get; set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        Image = GetComponent<Image>();
    }
}
