using UnityEngine;
using UnityEngine.Events;

public class ClickableButton : MonoBehaviour
{
    // Sự kiện sẽ xảy ra khi bạn bấm chuột vào nút này
    public UnityEvent onClick;

    private void OnMouseDown()
    {
        // Khi click chuột trái vào vật thể có Collider này, sự kiện sẽ kích hoạt
        onClick?.Invoke();
    }
}