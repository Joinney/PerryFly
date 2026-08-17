using UnityEngine;
using UnityEngine.InputSystem;

public class MusicToggle : MonoBehaviour
{
    [Header("Nguồn phát nhạc nền")]
    public AudioSource bgmAudioSource; // Kéo Menu_BGM vào đây

    [Header("Biểu tượng gạch chéo đè lên")]
    public GameObject muteOverlayIcon; // Kéo MuteIcon vào đây

    [Header("Âm thanh bấm nút (vẫn phát khi tắt nhạc)")]
    public AudioClip clickSound;
    private AudioSource clickAudioSource;

    private Collider2D col;
    private bool isMuted = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        clickAudioSource = GetComponent<AudioSource>();

        // Đọc trạng thái đã lưu (mặc định là 0 - có nhạc)
        isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        ApplyMusicState();
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            if (Camera.main == null) return;

            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));
            Vector2 worldPos = new Vector2(worldPos3D.x, worldPos3D.y);

            // Bấm trúng nút nốt nhạc
            if (col != null && col.OverlapPoint(worldPos))
            {
                ToggleMusic();
            }
        }
    }

    public void ToggleMusic()
    {
        // Vẫn phát tiếng click nút bình thường
        if (clickAudioSource != null && clickSound != null)
        {
            clickAudioSource.PlayOneShot(clickSound);
        }

        isMuted = !isMuted;
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusicState();
    }

    private void ApplyMusicState()
    {
        // Chỉ tắt/bật riêng tiếng nhạc nền
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = isMuted;
        }

        // Hiện biểu tượng gạch chéo khi tắt nhạc, ẩn đi khi bật nhạc
        if (muteOverlayIcon != null)
        {
            muteOverlayIcon.SetActive(isMuted);
        }
    }
}