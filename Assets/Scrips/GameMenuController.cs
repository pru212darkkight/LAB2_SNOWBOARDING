using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gameLevelPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject menuButtonsContainer;

    [Header("Option Panel Pages")]
    [SerializeField] private GameObject page1Container; // Container của trang 1
    [SerializeField] private GameObject page2Container; // Container của trang 2
    [SerializeField] private Button nextPageButton; // Nút next page (phải)
    [SerializeField] private Button prevPageButton; // Nút previous page (trái)
    private int currentPage = 1; // Trang hiện tại (1 hoặc 2)

    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    [Header("Panel Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button okOptionButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    [Header("Level Scene Names")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string level3SceneName = "Level3";


    private void Start()
    {
        // Đăng ký sự kiện click cho các nút menu
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClick);

        if (optionButton != null)
            optionButton.onClick.AddListener(OnOptionButtonClick);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClick);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClick);

        if (okOptionButton != null)
            okOptionButton.onClick.AddListener(OnOkOptionButtonClick);

        // Đăng ký sự kiện cho các nút chuyển trang
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(OnNextPageClick);

        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(OnPrevPageClick);

        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel(level1SceneName));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel(level2SceneName));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => LoadLevel(level3SceneName));

        // Đảm bảo các panel được ẩn khi bắt đầu
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(false);

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            InitializeOptionPages();
        }

        if (menuButtonsContainer != null)
            menuButtonsContainer.SetActive(true);
    }

    private void InitializeOptionPages()
    {
        if (page1Container != null)
            page1Container.SetActive(true);

        if (page2Container != null)
            page2Container.SetActive(false);

        if (prevPageButton != null)
            prevPageButton.gameObject.SetActive(false);

        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(true);

        currentPage = 1;
    }

    private void OnNextPageClick()
    {
        if (currentPage == 1)
        {
            if (page1Container != null)
                page1Container.SetActive(false);
            if (page2Container != null)
                page2Container.SetActive(true);

            currentPage = 2;

            if (prevPageButton != null)
                prevPageButton.gameObject.SetActive(true);
            if (nextPageButton != null)
                nextPageButton.gameObject.SetActive(false);
        }
    }

    private void OnPrevPageClick()
    {
        if (currentPage == 2)
        {
            if (page1Container != null)
                page1Container.SetActive(true);
            if (page2Container != null)
                page2Container.SetActive(false);

            currentPage = 1;

            if (prevPageButton != null)
                prevPageButton.gameObject.SetActive(false);
            if (nextPageButton != null)
                nextPageButton.gameObject.SetActive(true);
        }
    }

    private void OnPlayButtonClick()
    {
        // Hiện panel game level
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(true);
    }

    private void OnOptionButtonClick()
    {
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(false);

        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            InitializeOptionPages(); // Reset về trang đầu tiên khi mở option
        }

        if (menuButtonsContainer != null)
            menuButtonsContainer.SetActive(false);
    }

    private void OnOkOptionButtonClick()
    {
        // Ẩn panel option và hiện lại các nút menu
        if (optionPanel != null)
            optionPanel.SetActive(false);

        if (menuButtonsContainer != null)
            menuButtonsContainer.SetActive(true);
    }

    private void OnExitButtonClick()
    {
        // Thoát game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnCloseButtonClick()
    {
        // Chỉ ẩn panel game level
        if (gameLevelPanel != null)
            gameLevelPanel.SetActive(false);
    }

    private void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi destroy object
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClick);

        if (optionButton != null)
            optionButton.onClick.RemoveListener(OnOptionButtonClick);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClick);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseButtonClick);

        if (okOptionButton != null)
            okOptionButton.onClick.RemoveListener(OnOkOptionButtonClick);

        if (nextPageButton != null)
            nextPageButton.onClick.RemoveListener(OnNextPageClick);

        if (prevPageButton != null)
            prevPageButton.onClick.RemoveListener(OnPrevPageClick);

        if (level1Button != null)
            level1Button.onClick.RemoveAllListeners();

        if (level2Button != null)
            level2Button.onClick.RemoveAllListeners();

        if (level3Button != null)
            level3Button.onClick.RemoveAllListeners();
    }
}