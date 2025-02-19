using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioClip[] clips;
    [SerializeField] AudioSource source;
    [SerializeField] Slider slider;

    void Awake()
    {
        slider.value = PlayerPrefs.GetFloat("Volume", 1);
    }
    private void OnDisable()
    {
    }
    private void OnEnable()
    {
    }
    public void SetVolume()
    {
        source.volume = slider.value;
        PlayerPrefs.SetFloat("Volume", slider.value);
    }
}
