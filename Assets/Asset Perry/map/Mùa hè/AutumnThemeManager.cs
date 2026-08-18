using UnityEngine;

public class AutumnThemeManager : MonoBehaviour
{
    [Header("Tài nguyên Mùa Thu (Autumn Assets)")]
    public Sprite autumnBackgroundSprite;      // Kéo Sprite hình nền mùa thu vào đây
    public AudioClip autumnMusic;              // Kéo AudioClip nhạc mùa thu vào đây
    public ParticleSystem fallingLeavesEffect; // (Tùy chọn) Kéo hiệu ứng lá rơi vào nếu có

    [Header("Component trong Scene")]
    public SpriteRenderer backgroundRenderer;  // Kéo SpriteRenderer của Background vào
    public AudioSource bgmAudioSource;         // Kéo BGM AudioSource vào

    void Start()
    {
        ApplyAutumnTheme();
    }

    public void ApplyAutumnTheme()
    {
        // 1. Gán ảnh nền mùa thu
        if (backgroundRenderer != null && autumnBackgroundSprite != null)
        {
            backgroundRenderer.sprite = autumnBackgroundSprite;
        }

        // 2. Cấu hình và phát BGM mùa thu
        if (bgmAudioSource != null && autumnMusic != null)
        {
            bgmAudioSource.clip = autumnMusic;
            bgmAudioSource.loop = true;

            // Đọc trạng thái tắt/bật tiếng từ Menu
            bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            bgmAudioSource.mute = isMuted;

            bgmAudioSource.Play();
        }

        // 3. Kích hoạt hiệu ứng đặc trưng (nếu có)
        if (fallingLeavesEffect != null && !fallingLeavesEffect.isPlaying)
        {
            fallingLeavesEffect.Play();
        }

        Debug.Log(">>> Đã load thành công chủ đề: Autumn (Mùa Thu)");
    }
}