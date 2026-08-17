using UnityEngine;
using UnityEngine.UI;

public class HelpTutorialController : MonoBehaviour
{
    [Header("UI Panels & Images")]
    public GameObject tutorialPanel;   // Bảng popup hướng dẫn
    public Image tutorialImageDisplay; // Image hiển thị nội dung trang

    [Header("Danh sách 3 trang hướng dẫn")]
    public Sprite[] tutorialPages;     // Kéo 3 ảnh 11, 22, 33 vào đây

    [Header("Buttons")]
    public Button nextButton;          // Nút Next
    public Button backButton;          // Nút Back
    public Button closeButton;         // Nút đóng (X hoặc OK)

    [Header("Audio")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    private int currentPageIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Luôn ẩn bảng hướng dẫn khi mới vào Menu
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // Gắn sự kiện cho các nút UI
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (backButton != null) backButton.onClick.AddListener(PrevPage);
        if (closeButton != null) closeButton.onClick.AddListener(CloseTutorial);
    }

    // Hàm mở bảng hướng dẫn (gọi khi bấm nút ?)
    public void OpenTutorial()
    {
        PlaySound();
        currentPageIndex = 0;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        UpdatePageDisplay();
    }

    public void NextPage()
    {
        PlaySound();
        if (currentPageIndex < tutorialPages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
        else
        {
            CloseTutorial(); // Hết trang 3 thì đóng popup
        }
    }

    public void PrevPage()
    {
        PlaySound();
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }

    public void CloseTutorial()
    {
        PlaySound();
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void UpdatePageDisplay()
    {
        if (tutorialPages == null || tutorialPages.Length == 0 || tutorialImageDisplay == null) return;

        // Cập nhật sprite hiển thị theo trang hiện tại
        tutorialImageDisplay.sprite = tutorialPages[currentPageIndex];

        // Ẩn/Hiện nút Back (ở trang đầu tiên thì ẩn Back)
        if (backButton != null)
        {
            backButton.gameObject.SetActive(currentPageIndex > 0);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}