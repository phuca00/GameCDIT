using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip fruitClip;
    public AudioClip checkpointClip;
    public AudioClip damageClip;
    public AudioClip overClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    // ===========================
    // PLAY SFX
    // ===========================
    public void PlayJump() => sfxSource.PlayOneShot(jumpClip);
    public void PlayFruit() => sfxSource.PlayOneShot(fruitClip);
    public void PlayCheckpoint() => sfxSource.PlayOneShot(checkpointClip);
    public void PlayDamage() => sfxSource.PlayOneShot(damageClip);
    public void PlayOver() => sfxSource.PlayOneShot(overClip);

    // ===========================
    // VOLUME CONTROL
    // ===========================
    public void SetBGMVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("bgmVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("sfxVolume", value);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        bgmSource.volume = PlayerPrefs.GetFloat("bgmVolume", 0.5f);
        sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume", 0.7f);
    }
}
