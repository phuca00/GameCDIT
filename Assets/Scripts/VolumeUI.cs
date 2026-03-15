using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    public Slider bgmSlider;   // Nhạc nền
    public Slider sfxSlider;   // Âm hiệu ứng

    void Start()
    {
        // Load volume đã lưu
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.7f);

        // Gắn event
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    void OnBGMChanged(float value)
    {
        AudioManager.instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("bgmVolume", value);
    }

    void OnSFXChanged(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("sfxVolume", value);
    }
}
