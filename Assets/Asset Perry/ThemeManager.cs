using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [System.Serializable]
    public class MapThemeData
    {
        public string themeName;            // "Spring", "Summer", "Autumn", "Winter"
        public Sprite backgroundSprite;     // Hình nền của mùa đó
        public AudioClip backgroundMusic;    // Nhạc nền của mùa đó
    }

    [Header("Danh sách 4 Mùa")]
    public MapThemeData[] themes;

    [Header("Các đối tượng áp dụng trong Scene")]
    public SpriteRenderer backgroundRenderer; // Kéo SpriteRenderer của hình nền vào
    public AudioSource bgmAudioSource;        // Kéo BGM_Manager vào

    void Start()
    {
        // Lấy tên mùa người chơi đã chọn từ MapScene (mặc định là Spring)
        string selectedTheme = PlayerPrefs.GetString("SelectedMapTheme", "Spring");
        ApplyTheme(selectedTheme);
    }

    public void ApplyTheme(string themeName)
    {
        foreach (var theme in themes)
        {
            if (theme.themeName.Equals(themeName, System.StringComparison.OrdinalIgnoreCase))
            {
                // Đổi ảnh nền
                if (backgroundRenderer != null && theme.backgroundSprite != null)
                {
                    backgroundRenderer.sprite = theme.backgroundSprite;
                }

                // Đổi nhạc nền và phát
                if (bgmAudioSource != null && theme.backgroundMusic != null)
                {
                    bgmAudioSource.clip = theme.backgroundMusic;
                    bgmAudioSource.loop = true;

                    // Kiểm tra trạng thái tắt/bật tiếng đã lưu từ Menu
                    bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
                    bgmAudioSource.mute = isMuted;

                    bgmAudioSource.Play();
                }

                Debug.Log(">>> Đã load thành công chủ đề: " + themeName);
                return;
            }
        }
    }
}