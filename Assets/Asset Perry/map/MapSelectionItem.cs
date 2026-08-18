using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MapSelectionItem : MonoBehaviour
{
    [Header("Tên Mùa (Spring, Summer, Autumn, Winter)")]
    public string mapThemeName = "Spring";

    [Header("Yêu cầu Mở khóa Điểm")]
    public int requiredScore = 0;              // Mùa Xuân: 0, Mùa Hè: 200, Mùa Thu: 400, Mùa Đông: 600
    public string previousThemeKey = "";        // Tên mùa trước (Ví dụ: Mùa Hè ghi "Spring")
    public GameObject lockIcon;                 // GameObject Icon Ổ khóa (nếu có)

    [Header("Tên Scene chuyển đến")]
    public string targetSceneName = "SpringScene";

    [Header("Âm thanh bấm (tùy chọn)")]
    public AudioClip selectSound;
    public AudioClip lockedSound;               // Tiếng báo khóa (khi chưa đủ điểm)
    
    private AudioSource audioSource;
    private Collider2D myCollider;
    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (myCollider == null)
        {
            Debug.LogError(">>> CHƯA CÓ COLLIDER trên: " + gameObject.name);
        }

        CheckUnlockStatus();
    }

    // Kiểm tra điều kiện đủ điểm để mở map
    public void CheckUnlockStatus()
    {
        // Mùa Xuân không yêu cầu điểm (previousThemeKey rỗng hoặc requiredScore <= 0)
        if (string.IsNullOrEmpty(previousThemeKey) || requiredScore <= 0)
        {
            isUnlocked = true;
        }
        else
        {
            // Lấy HighScore của mùa trước
            int previousHighScore = PlayerPrefs.GetInt("HighScore_" + previousThemeKey, 0);
            isUnlocked = previousHighScore >= requiredScore;
        }

        // Tắt hẳn Collider nếu chưa mở khóa để chặn va chạm/click
        if (myCollider != null)
        {
            myCollider.enabled = isUnlocked;
        }

        // Bật/Tắt icon khóa hiển thị trên UI
        if (lockIcon != null)
        {
            lockIcon.SetActive(!isUnlocked);
        }

        // Làm mờ hình ảnh nếu chưa mở khóa
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isUnlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.8f);
        }

        Debug.Log($" Map [{mapThemeName}] | Trạng thái: {(isUnlocked ? "ĐÃ MỞ" : "ĐANG KHÓA")} | Yêu cầu: {requiredScore}đ từ [{previousThemeKey}]");
    }

    void Update()
    {
        // CHẶN TUYỆT ĐỐI: Nếu chưa mở khóa thì không xử lý nhận diện click/chạm
        if (!isUnlocked) return;

        // Kiểm tra click chuột trái hoặc chạm màn hình
        bool isPressed = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            isPressed = true;
        else if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            isPressed = true;

        if (isPressed)
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Vector2 mouseScreenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : Pointer.current.position.ReadValue();
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);

            // Bắn tia kiểm tra xem có trúng collider của đối tượng này không
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null && (hit.collider == myCollider || hit.collider.transform.IsChildOf(transform)))
            {
                Debug.Log(">>> ĐÃ MỞ MAP: " + mapThemeName + " -> Chuyển đến: " + targetSceneName);
                SelectThisMap();
            }
        }
    }

    void SelectThisMap()
    {
        PlayerPrefs.SetString("SelectedMapTheme", mapThemeName);
        PlayerPrefs.Save();

        if (audioSource != null && selectSound != null)
        {
            audioSource.PlayOneShot(selectSound);
            Invoke(nameof(LoadScene), 0.15f);
        }
        else
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("LỖI: Chưa tìm thấy Scene '" + targetSceneName + "' trong Build Settings!");
        }
    }
}