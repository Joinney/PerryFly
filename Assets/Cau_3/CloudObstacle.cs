using System.Collections;
using UnityEngine;

public class CloudObstacle : MonoBehaviour
{
    [Header("Sprite Trạng Thái")]
    public Sprite normalCloudSprite;     // Kéo sprite mây thường (bemay.png)
    public Sprite lightningCloudSprite;  // Kéo sprite mây sấm sét (bemay2.png)

    [Header("Thời gian chuyển đổi (giây)")]
    public float normalDuration = 2.0f;     // Thời gian ở dạng thường
    public float lightningDuration = 1.5f;  // Thời gian ở dạng sấm sét

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Collider2D damageCollider;       // Collider gây sát thương khi có sét

    private bool isLightning = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (damageCollider == null)
            damageCollider = GetComponent<Collider2D>();

        StartCoroutine(SwitchStateRoutine());
    }

    IEnumerator SwitchStateRoutine()
    {
        while (true)
        {
            // 1. Trạng thái Mây Thường (Không gây hại)
            SetLightningState(false);
            yield return new WaitForSeconds(normalDuration);

            // 2. Trạng thái Mây Sấm Sét (Gây GameOver)
            SetLightningState(true);
            yield return new WaitForSeconds(lightningDuration);
        }
    }

    void SetLightningState(bool lightning)
    {
        isLightning = lightning;

        if (isLightning)
        {
            if (lightningCloudSprite != null)
                spriteRenderer.sprite = lightningCloudSprite;

            // Bật collider gây chết người
            if (damageCollider != null)
                damageCollider.enabled = true;
        }
        else
        {
            if (normalCloudSprite != null)
                spriteRenderer.sprite = normalCloudSprite;

            // Tắt collider sát thương ở trạng thái thường
            if (damageCollider != null)
                damageCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Perry chạm vào lúc đang có sấm sét
        if (isLightning && collision.CompareTag("Player"))
        {
            PerryController perry = collision.GetComponent<PerryController>();
            if (perry != null)
            {
                // Gọi hàm chết / GameOver có sẵn của Perry
                // (Tùy hàm trong PerryController.cs: Die(), GameOver(), hoặc TakeDamage())
                collision.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}