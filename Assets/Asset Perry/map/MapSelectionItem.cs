using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MapSelectionItem : MonoBehaviour
{
    [Header("Tên Mùa (Spring, Summer, Autumn, Winter)")]
    public string mapThemeName = "Spring";

    [Header("Tên Scene chuyển đến")]
    public string targetSceneName = "SpringScene";

    [Header("Âm thanh bấm (tùy chọn)")]
    public AudioClip selectSound;
    private AudioSource audioSource;
    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (myCollider == null)
        {
            Debug.LogError(">>> CHƯA CÓ COLLIDER trên: " + gameObject.name);
        }
    }

    void Update()
    {
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
                Debug.Log(">>> ĐÃ CLICK TRÚNG MAP: " + mapThemeName + " -> Chuyển đến: " + targetSceneName);
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