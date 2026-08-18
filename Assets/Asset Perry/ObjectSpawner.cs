using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs Cây và Hoa")]
    public List<GameObject> objectsToSpawn; 

    [Header("Prefabs Coin")]
    public GameObject coinGroupTopPrefab;    
    public GameObject coinGroupBottomPrefab; 
    public float coinTopSpawnY = 0f;       
    public float coinBottomSpawnY = 1.2f;  

    [Header("Prefab Cột Điện (Codien)")]
    public GameObject codienPrefab;         // Kéo Prefab codien_0 vào đây
    public float codienTopSpawnY = 2.5f;    // Độ cao cột điện ở TRÊN cao
    public float codienBottomSpawnY = -1.2f;// Độ cao cột điện ở DƯỚI mặt đất

    [Header("Prefab Đám Mây Sấm Sét (Cloud Obstacle)")]
    public GameObject cloudPrefab;          // Kéo Prefab Cloud_Obstacle vào đây
    [Range(0f, 1f)]
    public float cloudSpawnChance = 0.5f;   // Tỷ lệ xuất hiện (0.5 = 50%)
    public float cloudMinSpawnY = 0.5f;     // Độ cao tối thiểu
    public float cloudMaxSpawnY = 2.5f;     // Độ cao tối đa

    [Header("Cấu Hình Lặp Lại Map")]
    public Transform cameraTransform;       
    public float spawnIntervalDistance = 14.3f; 
    public float spawnAheadDistance = 20f;    
    public float fixedSpawnY = 0f;        

    [Header("Chế Độ Luân Phiên")]
    public bool useAlternatePattern = true; // Tích chọn để luân phiên đều đặn

    private float nextSpawnX;
    private bool isNextSpawnTop = true;

    void Start()
    {
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
        if (cameraTransform == null) return;

        // Khi Camera tiến gần mốc tiếp theo, sinh đợt mới
        if (cameraTransform.position.x + spawnAheadDistance > nextSpawnX)
        {
            SpawnMapAndElements();
        }
    }

    void SpawnMapAndElements()
    {
        // 1. Sinh đất / cảnh nền ngẫu nhiên
        if (objectsToSpawn != null && objectsToSpawn.Count > 0)
        {
            int randomIndex = Random.Range(0, objectsToSpawn.Count);
            GameObject prefabToSpawn = objectsToSpawn[randomIndex];
            if (prefabToSpawn != null)
            {
                GameObject spawnedMap = Instantiate(prefabToSpawn, new Vector3(nextSpawnX, fixedSpawnY, 0f), Quaternion.identity);
                Destroy(spawnedMap, 15f);
            }
        }

        // 2. Sinh Coin và Cột Điện luân phiên đều đặn
        if (coinGroupTopPrefab != null && coinGroupBottomPrefab != null)
        {
            GameObject selectedCoinPrefab;
            float targetCoinY;
            float targetCodienY;

            bool spawnTop = useAlternatePattern ? isNextSpawnTop : (Random.Range(0, 2) == 0);

            if (spawnTop)
            {
                // Đợt này: Coin ở TRÊN -> Cột điện ở DƯỚI
                selectedCoinPrefab = coinGroupTopPrefab;
                targetCoinY = coinTopSpawnY;
                targetCodienY = codienBottomSpawnY;
            }
            else
            {
                // Đợt này: Coin ở DƯỚI -> Cột điện ở TRÊN
                selectedCoinPrefab = coinGroupBottomPrefab;
                targetCoinY = coinBottomSpawnY;
                targetCodienY = codienTopSpawnY;
            }

            // Sinh cụm Coin
            GameObject spawnedCoins = Instantiate(selectedCoinPrefab, new Vector3(nextSpawnX, targetCoinY, 0f), Quaternion.identity);
            Destroy(spawnedCoins, 15f);

            // Sinh Cột Điện lệch nửa nhịp (giữa 2 cụm coin)
            if (codienPrefab != null)
            {
                float codienX = nextSpawnX + (spawnIntervalDistance * 0.5f);
                GameObject spawnedCodien = Instantiate(codienPrefab, new Vector3(codienX, targetCodienY, 0f), Quaternion.identity);
                Destroy(spawnedCodien, 15f);
            }

            // Đổi trạng thái cho nhịp tiếp theo
            if (useAlternatePattern)
            {
                isNextSpawnTop = !isNextSpawnTop;
            }
        }

        // 3. Sinh Đám mây sấm sét ngẫu nhiên trên không
        if (cloudPrefab != null && Random.value <= cloudSpawnChance)
        {
            float randomCloudY = Random.Range(cloudMinSpawnY, cloudMaxSpawnY);
            // Đặt đám mây lệch khoảng 1/4 nhịp để không đè trực tiếp lên cột điện
            float cloudX = nextSpawnX + (spawnIntervalDistance * 0.25f);
            GameObject spawnedCloud = Instantiate(cloudPrefab, new Vector3(cloudX, randomCloudY, 0f), Quaternion.identity);
            Destroy(spawnedCloud, 15f);
        }

        nextSpawnX += spawnIntervalDistance;
    }
}