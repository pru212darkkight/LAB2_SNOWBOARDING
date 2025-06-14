using UnityEngine;
using TMPro;  // Sử dụng TextMeshPro cho chất lượng text tốt hơn

public class SpeedDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController player;  // Tham chiếu đến Player
    [SerializeField] private TextMeshProUGUI speedText;  // Component Text để hiển thị

    private void Update()
    {
        if (player != null && speedText != null)
        {
            float currentSpeed = player.GetCurrentSpeed();
            speedText.text = $"Speed: {currentSpeed:F0} km/h";  // Hiển thị tốc độ với 1 số thập phân
        }
    }
}