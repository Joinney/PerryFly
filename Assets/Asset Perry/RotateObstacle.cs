using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    [Header("Tốc độ xoay (độ/giây)")]
    public float rotateSpeed = 150f; // Số dương: xoay ngược chiều kim đồng hồ, Số âm: xoay theo chiều kim đồng hồ

    void Update()
    {
        // Xoay tròn đều quanh tâm trục Z
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}