using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameLevelPanel; // Panel chọn level

    [Header("Buttons")]
    [SerializeField] private Button playButton; // Nút Play
    [SerializeField] private Button closeButton; // Nút Close để đóng panel
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    [Header("Level Scene Names")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string level3SceneName = "Level3";

    private void Start()
    {
        // Đăng ký sự kiện click cho các nút
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClick);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClick);

        // Đăng ký sự kiện cho các nút level
        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel(level1SceneName));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel(level2SceneName));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => LoadLevel(level3SceneName));

        // Đảm bảo panel được ẩn khi bắt đầu
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(false);
    }

    private void OnPlayButtonClick()
    {
        // Hiện panel game level
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(true);
    }

    private void OnCloseButtonClick()
    {
        // Chỉ ẩn panel game level
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(false);
    }

    private void LoadLevel(string sceneName)
    {
        // Có thể thêm animation loading hoặc transition ở đây
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi destroy object
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClick);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClick);

        if (level1Button != null)
            level1Button.onClick.RemoveAllListeners();

        if (level2Button != null)
            level2Button.onClick.RemoveAllListeners();

        if (level3Button != null)
            level3Button.onClick.RemoveAllListeners();
    }
}