using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Khai báo biến để kéo thả nhân vật Perry từ Unity vào
    public GameObject player; 

    // Biến lưu khoảng cách ban đầu giữa camera và player
    private float distanceToPlayer;

    void Start()
    {
        if (player != null)
        {
            distanceToPlayer = transform.position.x - player.transform.position.x;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            float playerX = player.transform.position.x;
            Vector3 newCameraPos = transform.position; 
            
            // Camera chỉ đi theo Player theo trục X
            newCameraPos.x = playerX + distanceToPlayer; 
            
            transform.position = newCameraPos;
        }
    }
}