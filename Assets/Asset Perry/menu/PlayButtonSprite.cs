using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayButtonSprite : MonoBehaviour
{
    [Header("Tên Scene chuyển đến")]
    public string targetSceneName = "MapScene"; // Tên scene chọn map

    [Header("Âm thanh bấm nút (tùy chọn)")]
    public AudioClip clickSound;
    private AudioSource audioSource;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Bắt sự kiện click chuột trái hoặc chạm màn hình
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPos3D = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -cam.transform.position.z));
            Vector2 worldPos = new Vector2(worldPos3D.x, worldPos3D.y);

            // Kiểm tra click trúng nút Play
            if (col != null && col.OverlapPoint(worldPos))
            {
                Debug.Log(">>> Đang chuyển sang: " + targetSceneName);

                if (audioSource != null && clickSound != null)
                {
                    audioSource.PlayOneShot(clickSound);
                    Invoke(nameof(LoadTargetScene), 0.15f);
                }
                else
                {
                    LoadTargetScene();
                }
            }
        }
    }

    void LoadTargetScene()
    {
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("Chưa tìm thấy Scene tên là '" + targetSceneName + "' trong Build Settings!");
        }
    }
}