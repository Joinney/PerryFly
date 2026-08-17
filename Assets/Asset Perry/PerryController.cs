using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PerryController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 5f; 
    public float jumpForce = 6f; 

    [Header("Particle & Explosion Prefabs")]
    public ParticleSystem jetpackEffect; 
    public GameObject groundExplosionPrefab; // Prefab nổ dưới đất
    public GameObject airExplosionPrefab;    // Prefab nổ trên không

    [Header("Audio Settings")]
    public AudioClip coinSound;          // File smw_coin
    public AudioClip hitSound;           // File GAMEOVER
    public AudioClip countSound;         // File smw_bonus_game (âm thanh nhảy số)
    public AudioClip buttonClickSound;   // Tiếng click bấm nút Restart / Menu
    private AudioSource audioSource;     

    [Header("Score Settings")]
    public int coinCount = 0;            
    public TMP_Text coinTextUI;          // Text hiển thị góc màn hình lúc chơi

    [Header("Game Over UI Settings")]
    public GameObject gameOverPanel;     // GameObject cha GameOverPanel
    public Image gameOverImage;          // Component Image của GameOverPanel
    public Sprite gameOverIntroSprite;   // Ảnh logo_gameover_2 (chỉ có chữ Game Over)
    public Sprite gameOverScoreSprite;   // Ảnh logo_gameover_4 (có khung Your Score & High Score)
    public TMP_Text yourScoreText;       // Text hiển thị Your Score
    public TMP_Text highScoreText;       // Text hiển thị High Score
    public float scoreCountDuration = 1.0f; // Thời gian hiệu ứng nhảy số (giây)

    [Header("Game Over Buttons")]
    public Button restartButton;         // Kéo RestartButton vào đây
    public Button homeButton;            // Kéo HomeButton vào đây

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isGrounded = true;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (jetpackEffect != null)
        {
            jetpackEffect.Stop(); 
        }

        // Ẩn bảng Game Over và các nút lúc vừa vào game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (homeButton != null)
        {
            homeButton.gameObject.SetActive(false);
            homeButton.onClick.AddListener(OnHomeClicked);
        }

        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead) return;

        // 1. Kiểm tra giữ chuột / chạm màn hình
        bool isHoldingClick = Pointer.current != null && Pointer.current.press.isPressed;

        // 2. Tính toán vận tốc trục Y
        float targetVelocityY = (rb != null) ? rb.linearVelocity.y : 0f;

        if (isHoldingClick)
        {
            targetVelocityY = jumpForce; // Lực bay đẩy lên

            if (anim != null)
            {
                anim.SetBool("isJumping", true);
            }

            if (jetpackEffect != null && !jetpackEffect.isPlaying)
            {
                jetpackEffect.Play();
            }
        }
        else
        {
            if (jetpackEffect != null && jetpackEffect.isPlaying)
            {
                jetpackEffect.Stop();
            }
        }

        // 3. Giữ tốc độ chạy ngang không bị tụt khi bay
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(runSpeed, targetVelocityY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("floor"))
        {
            isGrounded = true;
            if (anim != null)
            {
                anim.SetBool("isJumping", false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("floor"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        // 1. Nhặt Coin
        if (collision.CompareTag("Coin"))
        {
            coinCount++;
            UpdateCoinUI();

            if (audioSource != null && coinSound != null)
            {
                audioSource.PlayOneShot(coinSound);
            }

            Destroy(collision.gameObject);
        }

        // 2. Chạm Cột Điện
        if (collision.CompareTag("codien"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (jetpackEffect != null)
        {
            jetpackEffect.Stop();
        }

        if (col != null)
        {
            col.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        GameObject selectedExplosion = isGrounded ? groundExplosionPrefab : airExplosionPrefab;
        if (selectedExplosion == null)
        {
            selectedExplosion = (groundExplosionPrefab != null) ? groundExplosionPrefab : airExplosionPrefab;
        }

        if (selectedExplosion != null)
        {
            Instantiate(selectedExplosion, transform.position, Quaternion.identity);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Lưu High Score
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (coinCount > currentHighScore)
        {
            currentHighScore = coinCount;
            PlayerPrefs.SetInt("HighScore", currentHighScore);
            PlayerPrefs.Save();
        }

        StartCoroutine(GameOverSequenceRoutine());
    }

    private IEnumerator GameOverSequenceRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        if (gameOverPanel != null)
        {
            // BƯỚC 1: Hiện bảng logo_gameover_2
            if (gameOverImage != null && gameOverIntroSprite != null)
            {
                gameOverImage.sprite = gameOverIntroSprite;
            }

            if (yourScoreText != null) yourScoreText.text = "";
            if (highScoreText != null) highScoreText.text = "";

            if (restartButton != null) restartButton.gameObject.SetActive(false);
            if (homeButton != null) homeButton.gameObject.SetActive(false);

            gameOverPanel.SetActive(true);
        }

        yield return new WaitForSeconds(1.2f);

        // BƯỚC 2: Đổi sang logo_gameover_4
        if (gameOverImage != null && gameOverScoreSprite != null)
        {
            gameOverImage.sprite = gameOverScoreSprite;
        }

        // Nhảy số điểm
        yield return StartCoroutine(CountScoreRoutine(coinCount, PlayerPrefs.GetInt("HighScore", 0)));

        // BƯỚC 3: Hiện 2 nút Chơi lại và Về Menu sau khi nhảy số xong
        if (restartButton != null) restartButton.gameObject.SetActive(true);
        if (homeButton != null) homeButton.gameObject.SetActive(true);
    }

    private IEnumerator CountScoreRoutine(int targetYourScore, int targetHighScore)
    {
        float elapsedTime = 0f;
        int lastSoundScore = -1;

        while (elapsedTime < scoreCountDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / scoreCountDuration);

            int currentVal = Mathf.RoundToInt(Mathf.Lerp(0, targetYourScore, progress));
            int currentHighVal = Mathf.RoundToInt(Mathf.Lerp(0, targetHighScore, progress));

            if (yourScoreText != null) yourScoreText.text = currentVal.ToString();
            if (highScoreText != null) highScoreText.text = currentHighVal.ToString();

            if (currentVal != lastSoundScore && countSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(countSound, 0.4f);
                lastSoundScore = currentVal;
            }

            yield return null;
        }

        if (yourScoreText != null) yourScoreText.text = targetYourScore.ToString();
        if (highScoreText != null) highScoreText.text = targetHighScore.ToString();
    }

    public void OnRestartClicked()
    {
        PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnHomeClicked()
    {
        PlayButtonClick();
        SceneManager.LoadScene("MenuScene");
    }

    private void PlayButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    void UpdateCoinUI()
    {
        if (coinTextUI != null)
        {
            coinTextUI.text = "Coins: " + coinCount;
        }
    }
}