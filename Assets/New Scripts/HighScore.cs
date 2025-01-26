using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class HighScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] highscoreTexts;
    [SerializeField] GameObject restartTipText;
    void Start()
    {
        List<HighscoreEntry> highscores = SaveSystem.DisplayHighscores(SaveSystem.highscoreSave);

        int index = 0;
        if (highscores != null)
        {
            foreach (HighscoreEntry entry in highscores)
            {
                highscoreTexts[index].text = entry.playerName + " " + entry.score;

                if (MenuManager.Instance != null && entry.playerName == MenuManager.Instance.playerName && MenuManager.Instance.score == entry.score)
                {
                    StartCoroutine(BlinkText(highscoreTexts[index]));
                }

                index++;
                if (index > highscoreTexts.Length - 1)
                {
                    break;
                }
            }
        }
        if (!Regex.IsMatch(MenuManager.Instance.playerName, @"^[a-zA-Z]+$"))
        {
            restartTipText.SetActive(false);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Regex.IsMatch(MenuManager.Instance.playerName, @"^[a-zA-Z]+$"))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape) )
        {
            SceneManager.LoadScene(0);
        }
    }
    public void PlaySound(AudioClip clip)
    {
        MenuManager.Instance.PlaySound(clip);
    }

    IEnumerator BlinkText(TextMeshProUGUI textMeshProUGUI)
    {
        string originalText = textMeshProUGUI.text;
        while (true)
        {
            textMeshProUGUI.text = null;
            yield return new WaitForSeconds(0.5f);
            textMeshProUGUI.text = originalText;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
