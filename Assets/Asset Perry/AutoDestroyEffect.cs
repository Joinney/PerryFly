using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public float delay = 1.2f; // Thời gian chạy xong toàn bộ 22 frame nổ
    void Start()
    {
        Destroy(gameObject, delay);
    }
}