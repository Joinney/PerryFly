using UnityEngine;
using UnityEngine.InputSystem;

public class PerryController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 5f; 
    public float jumpForce = 6f; 

    [Header("Particle Settings")]
    public ParticleSystem jetpackEffect; 

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (jetpackEffect != null)
        {
            jetpackEffect.Stop(); 
        }
    }

    void Update()
    {
        // Tự động chạy sang phải
        rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);

        // Đẩy lên khi giữ chuột/chạm
        bool isHoldingClick = Pointer.current != null && Pointer.current.press.isPressed;

        if (isHoldingClick)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("isJumping", true);

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
        if (collision.gameObject.name == "floor")
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "floor")
        {
            isGrounded = false;
        }
    }
}