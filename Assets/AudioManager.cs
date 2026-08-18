using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Nguồn nhạc nền (kéo BGM_Manager vào đây)
    public AudioSource sfxSource;   // Nguồn hiệu ứng âm thanh

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        // Đảm bảo AudioManager tồn tại xuyên suốt các Scene không bị mất
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tải lại mức âm lượng đã lưu trước đó của người chơi
        if (musicSlider != null && musicSource != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSource.volume = musicSlider.value;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null && sfxSource != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSource.volume = sfxSlider.value;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        PlayerPrefs.SetFloat("MusicVolume", volume); // Lưu lại giá trị
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        PlayerPrefs.SetFloat("SFXVolume", volume); // Lưu lại giá trị
    }
}