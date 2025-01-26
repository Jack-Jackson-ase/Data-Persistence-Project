using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public int highScore;
    public string playerName = null;

    [SerializeField] TMP_InputField nameField;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] Slider volumeSlider;

    [SerializeField] AudioClip invalidOptionSound;

    [System.Serializable]
    public class VolumeData { public float volume; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            ChangePlayerName();
            StartCoroutine(BlinkingTitle());

            string path = SaveSystem.GetFilePath(SaveSystem.gameSettingsSave);
            if (File.Exists(path) == true)
            {
                VolumeData volumeData = SaveSystem.LoadData<VolumeData>(SaveSystem.gameSettingsSave);
                MenuManager.Instance.volume = volumeData.volume;
                volumeSlider.value = MenuManager.Instance.volume;
            }
        }
    }
    public void PlaySound(AudioClip clip)
    {
        MenuManager.Instance.PlaySound(clip);
    }
    public void StartGame()
    {
        if (playerName != null)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            nameField.text = "Invalid Name";
            MenuManager.Instance.PlaySound(invalidOptionSound);
        }
    }
    public void OpenLeaderboard()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
        VolumeData volumeData = new VolumeData { volume = MenuManager.Instance.volume};
        SaveSystem.SaveData(volumeData, SaveSystem.gameSettingsSave);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

    public void ChangePlayerName()
    {
        if (Regex.IsMatch(nameField.text, @"^[a-zA-Z]+$"))
        {
            playerName = nameField.text;
            nameField.text = playerName;
            MenuManager.Instance.playerName = nameField.text;
        }
        else
        {
            playerName = null;
        }
        Debug.Log("PlayerName Changed to " + playerName);
    }

    public void OptionsButtonPressed()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }

    public void CheckVolume()
    {
        MenuManager.Instance.volume = volumeSlider.value;
        VolumeData volumeData = new VolumeData { volume = MenuManager.Instance.volume };
        SaveSystem.SaveData(volumeData, SaveSystem.gameSettingsSave);
    }
    IEnumerator BlinkingTitle()
    {
        string titleText = title.text;
        while (SceneManager.GetActiveScene().name == "menu")
        {
            title.text = titleText;
            yield return new WaitForSeconds(2f);
            title.text = null;
            yield return new WaitForSeconds(1f);
        }
    }

    public void ClearData()
    {
        SaveSystem.ClearHighscores();
    }
}
