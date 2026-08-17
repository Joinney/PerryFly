using UnityEngine;
using UnityEngine.InputSystem;

public class HelpButtonTrigger : MonoBehaviour
{
    public HelpTutorialController helpController;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Nhận diện click chuột hoặc chạm ngón tay theo chuẩn New Input System
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            if (Camera.main == null) return;

            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // Kiểm tra con trỏ có nằm trong vùng va chạm của nút Help không
            if (col != null && col.OverlapPoint(worldPosition))
            {
                if (helpController != null)
                {
                    helpController.OpenTutorial();
                }
            }
        }
    }
}