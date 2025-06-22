using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Text sfxVolumeText;
    public Text musicVolumeText;

    //[Header("Mouse Settings")]
    //public Slider mouseSensitivitySlider;
    //public Text mouseSensitivityText;

    //[Header("Game Mode Settings")]
    //public Toggle toggleKeyboard;
    //public Toggle toggleCursor;

    public GameObject settingsPanel;

    private void Start()
    {
        // Khởi tạo audio settings
        if (AudioManager.Instance != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            UpdateVolumeTexts();
        }

        // Khởi tạo mouse sensitivity từ PlayerPrefs
        //float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        //mouseSensitivitySlider.value = savedSensitivity;
        //UpdateMouseSensitivityText();

        // Thêm listeners cho audio
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        //mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

    }

    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            UpdateVolumeTexts();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.optionButtonClickSound);
        }
    }



    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
            UpdateVolumeTexts();
        }
    }

    //public void OnMouseSensitivityChanged(float value)
    //{
    //    PlayerPrefs.SetFloat("MouseSensitivity", value);
    //    PlayerPrefs.Save();
    //    //UpdateMouseSensitivityText();
    //}

    private void UpdateMouseSensitivityText()
    {
        //mouseSensitivityText.text = $"{(mouseSensitivitySlider.value * 100):0}%";
    }

    private void UpdateVolumeTexts()
    {
        sfxVolumeText.text = $"{(sfxVolumeSlider.value * 100):0}%";
        musicVolumeText.text = $"{(musicVolumeSlider.value * 100):0}%";
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    //public static float GetMouseSensitivity()
    //{
    //    return PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    //}
}