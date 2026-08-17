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

        // Ẩn bảng Game Over lúc vừa vào game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead) return;

        // Tự động chạy sang phải
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }

        // Đẩy lên khi giữ chuột / chạm màn hình
        bool isHoldingClick = Pointer.current != null && Pointer.current.press.isPressed;

        if (isHoldingClick)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

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

        // Tắt hiệu ứng khói và va chạm
        if (jetpackEffect != null)
        {
            jetpackEffect.Stop();
        }

        if (col != null)
        {
            col.enabled = false;
        }

        // Dừng chuyển động nhân vật
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        // Kích hoạt hiệu ứng nổ phù hợp với trạng thái bay / chạm đất
        GameObject selectedExplosion = isGrounded ? groundExplosionPrefab : airExplosionPrefab;
        if (selectedExplosion == null)
        {
            selectedExplosion = (groundExplosionPrefab != null) ? groundExplosionPrefab : airExplosionPrefab;
        }

        if (selectedExplosion != null)
        {
            Instantiate(selectedExplosion, transform.position, Quaternion.identity);
        }

        // Ẩn hình nhân vật gốc
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Phát âm thanh va chạm / nổ
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Tính và lưu High Score
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (coinCount > currentHighScore)
        {
            currentHighScore = coinCount;
            PlayerPrefs.SetInt("HighScore", currentHighScore);
            PlayerPrefs.Save();
        }

        // Chạy quy trình hiển thị Game Over 2 bước
        StartCoroutine(GameOverSequenceRoutine());
    }

    // Quy trình hiển thị: Chờ nổ -> Hiện logo_gameover_2 -> Đổi sang logo_gameover_4 -> Nhảy số điểm
    private IEnumerator GameOverSequenceRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        if (gameOverPanel != null)
        {
            // BƯỚC 1: Hiện ảnh logo_gameover_2 (chưa có điểm)
            if (gameOverImage != null && gameOverIntroSprite != null)
            {
                gameOverImage.sprite = gameOverIntroSprite;
            }

            // Tạm thời ẩn chữ số điểm
            if (yourScoreText != null) yourScoreText.text = "";
            if (highScoreText != null) highScoreText.text = "";

            gameOverPanel.SetActive(true);
        }

        // Chờ 1.2 giây để người chơi nhìn bảng Game Over ban đầu
        yield return new WaitForSeconds(1.2f);

        // BƯỚC 2: Đổi sang ảnh logo_gameover_4 (có khung Your Score & High Score)
        if (gameOverImage != null && gameOverScoreSprite != null)
        {
            gameOverImage.sprite = gameOverScoreSprite;
        }

        // Chạy hiệu ứng số nhảy và phát âm thanh
        yield return StartCoroutine(CountScoreRoutine(coinCount, PlayerPrefs.GetInt("HighScore", 0)));

        // Chờ 2.5 giây cho người chơi xem kết quả trước khi tải lại game
        yield return new WaitForSeconds(2.5f);
        ReloadScene();
    }

    // Hiệu ứng số nhảy kèm âm thanh
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

            if (yourScoreText != null)
            {
                yourScoreText.text = currentVal.ToString();
            }

            if (highScoreText != null)
            {
                highScoreText.text = currentHighVal.ToString();
            }

            // Phát âm thanh khi giá trị điểm thay đổi
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

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateCoinUI()
    {
        if (coinTextUI != null)
        {
            coinTextUI.text = "Coins: " + coinCount;
        }
    }
}