using UnityEngine;

public class CoinPatternManager : MonoBehaviour
{
    public GameObject coinGroupTop;    // Kéo CoinGrouptop vào đây
    public GameObject coinGroupBottom; // Kéo CoinGroupbottom vào đây

    void OnEnable()
    {
        RandomizePattern();
    }

    // Hàm chọn ngẫu nhiên cụm trên hoặc dưới
    public void RandomizePattern()
    {
        // Tỉ lệ 50/50: 0 là chỉ hiện trên, 1 là chỉ hiện dưới
        int pattern = Random.Range(0, 2);

        if (pattern == 0)
        {
            coinGroupTop.SetActive(true);
            coinGroupBottom.SetActive(false);
        }
        else
        {
            coinGroupTop.SetActive(false);
            coinGroupBottom.SetActive(true);
        }

        // Bật lại các đồng coin con bên trong cụm được chọn (đề phòng vòng trước đã bị ăn)
        ResetCoins(coinGroupTop);
        ResetCoins(coinGroupBottom);
    }

    private void ResetCoins(GameObject parent)
    {
        foreach (Transform coin in parent.GetComponentsInChildren<Transform>(true))
        {
            if (coin != parent.transform)
            {
                coin.gameObject.SetActive(true);
            }
        }
    }
}