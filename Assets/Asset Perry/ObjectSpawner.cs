using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs Cây và Hoa")]
    // Kéo thả xuan1 và xuan2 (đã sửa pivot về 0) từ ô Project vào đây
    public List<GameObject> objectsToSpawn; 

    [Header("Cấu Hình Lặp Lại")]
    public Transform cameraTransform;       // Kéo Main Camera vào đây
    public float spawnIntervalDistance = 10.24f; // Sửa lại đúng bằng chiều dài thực tế của block đất (ví dụ: 10 hoặc 10.24)
    public float spawnAheadDistance = 15f;    // Sinh ra trước Camera để người chơi không thấy nó đột ngột hiện ra
    public float fixedSpawnY = -3.14f;        // Độ cao cố định để đất sinh ra khớp với chân của Perry (-3.14)

    private float nextSpawnX;

    void Start()
    {
        // Đồng bộ điểm xuất phát đầu tiên theo vị trí hiện tại của Camera để tránh lệch bản đồ
        if (cameraTransform != null)
        {
            nextSpawnX = cameraTransform.position.x + spawnIntervalDistance;
        }
        else
        {
            nextSpawnX = spawnIntervalDistance;
        }
    }

    void Update()
    {
        if (cameraTransform == null || objectsToSpawn.Count == 0) return;

        // Nếu Camera tiến tới gần điểm nextSpawnX, tiến hành tạo block mới phía trước
        if (cameraTransform.position.x + spawnAheadDistance > nextSpawnX)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // Chọn ngẫu nhiên xuan1 hoặc xuan2
        int randomIndex = Random.Range(0, objectsToSpawn.Count);
        GameObject prefabToSpawn = objectsToSpawn[randomIndex];

        // Tạo vật thể nối tiếp chính xác trên trục X, cố định trục Y nằm thẳng tắp
        GameObject spawnedObj = Instantiate(prefabToSpawn, new Vector3(nextSpawnX, fixedSpawnY, 0f), Quaternion.identity);

        // Tự động dọn dẹp các block cũ phía sau để tối ưu hóa bộ nhớ
        Destroy(spawnedObj, 12f);

        // Tăng khoảng cách để block tiếp theo nối đuôi khít sát vào block hiện tại
        nextSpawnX += spawnIntervalDistance;
    }
}