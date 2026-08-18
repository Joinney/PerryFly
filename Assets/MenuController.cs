using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject settingPanel; // Kéo SettingPanel vào đây

    // Hàm mở bảng Setting (gắn vào nút bánh răng)
    public void OpenSetting()
    {
        if (settingPanel != null)
            settingPanel.SetActive(true);
    }

    // Hàm đóng bảng Setting (gắn vào nút X)
    public void CloseSetting()
    {
        if (settingPanel != null)
            settingPanel.SetActive(false);
    }
}