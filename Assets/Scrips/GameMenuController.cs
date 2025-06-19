using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameMenuController : MonoBehaviour
{
    public static string LastPlayedScene { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject gameLevelPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject menuButtonsContainer;

    [Header("Option Panel Pages")]
    [SerializeField] private GameObject page1Container;
    [SerializeField] private GameObject page2Container;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    private int currentPage = 1;

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

    [Header("Star Display Images (Single)")]
    [SerializeField] private Image level1StarImageSingle;
    [SerializeField] private Image level2StarImageSingle;
    [SerializeField] private Image level3StarImageSingle;

    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    private void Start()
    {
        playButton?.onClick.AddListener(OnPlayButtonClick);
        optionButton?.onClick.AddListener(OnOptionButtonClick);
        exitButton?.onClick.AddListener(OnExitButtonClick);
        closeButton?.onClick.AddListener(OnCloseButtonClick);
        okOptionButton?.onClick.AddListener(OnOkOptionButtonClick);

        nextPageButton?.onClick.AddListener(OnNextPageClick);
        prevPageButton?.onClick.AddListener(OnPrevPageClick);

        level1Button?.onClick.AddListener(() => LoadLevel(level1SceneName));
        level2Button?.onClick.AddListener(() => LoadLevel(level2SceneName));
        level3Button?.onClick.AddListener(() => LoadLevel(level3SceneName));

        gameLevelPanel?.SetActive(false);
        optionPanel?.SetActive(false);
        menuButtonsContainer?.SetActive(true);

        InitializeOptionPages();
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        UpdateLevelButtonsState();
    }

    private void InitializeOptionPages()
    {
        page1Container?.SetActive(true);
        page2Container?.SetActive(false);
        prevPageButton?.gameObject.SetActive(false);
        nextPageButton?.gameObject.SetActive(true);
        currentPage = 1;
    }

    private void OnNextPageClick()
    {
        if (currentPage == 1)
        {
            page1Container?.SetActive(false);
            page2Container?.SetActive(true);
            currentPage = 2;

            prevPageButton?.gameObject.SetActive(true);
            nextPageButton?.gameObject.SetActive(false);
        }
    }

    private void OnPrevPageClick()
    {
        if (currentPage == 2)
        {
            page1Container?.SetActive(true);
            page2Container?.SetActive(false);
            currentPage = 1;

            prevPageButton?.gameObject.SetActive(false);
            nextPageButton?.gameObject.SetActive(true);
        }
    }

    private void OnPlayButtonClick()
    {
        gameLevelPanel?.SetActive(true);
    }

    private void OnOptionButtonClick()
    {
        gameLevelPanel?.SetActive(false);
        optionPanel?.SetActive(true);
        menuButtonsContainer?.SetActive(false);
        InitializeOptionPages();
    }

    private void OnOkOptionButtonClick()
    {
        optionPanel?.SetActive(false);
        menuButtonsContainer?.SetActive(true);
    }

    private void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnCloseButtonClick()
    {
        gameLevelPanel?.SetActive(false);
    }

    private void LoadLevel(string sceneName)
    {
        PlayerPrefs.SetString("LastPlayedScene", sceneName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateLevelButtonsState()
    {
        level2Button.interactable = LevelCompletionChecker.IsLevelCompleted(level1SceneName);
        level3Button.interactable = LevelCompletionChecker.IsLevelCompleted(level2SceneName);

        SetStarSprite(level1StarImageSingle, LevelCompletionChecker.GetStarCount(level1SceneName));
        SetStarSprite(level2StarImageSingle, LevelCompletionChecker.GetStarCount(level2SceneName));
        SetStarSprite(level3StarImageSingle, LevelCompletionChecker.GetStarCount(level3SceneName));
    }

    private void SetStarSprite(Image starImage, int starCount)
    {
        if (starImage == null) return;

        switch (starCount)
        {
            case 1:
                starImage.sprite = star1Sprite;
                break;
            case 2:
                starImage.sprite = star2Sprite;
                break;
            case 3:
                starImage.sprite = star3Sprite;
                break;
            default:
                starImage.sprite = star0Sprite;
                break;
        }
    }

    private void OnDestroy()
    {
        playButton?.onClick.RemoveListener(OnPlayButtonClick);
        optionButton?.onClick.RemoveListener(OnOptionButtonClick);
        exitButton?.onClick.RemoveListener(OnExitButtonClick);
        closeButton?.onClick.RemoveListener(OnCloseButtonClick);
        okOptionButton?.onClick.RemoveListener(OnOkOptionButtonClick);
        nextPageButton?.onClick.RemoveListener(OnNextPageClick);
        prevPageButton?.onClick.RemoveListener(OnPrevPageClick);

        level1Button?.onClick.RemoveAllListeners();
        level2Button?.onClick.RemoveAllListeners();
        level3Button?.onClick.RemoveAllListeners();
    }
}