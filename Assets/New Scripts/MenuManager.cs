using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

class JsonSaveHighscore
{
    public int highScore;
    public string playerName;
}

public class MenuManager : MonoBehaviour
{
    public string playerName;
    public int score;
    public float volume = 0.5f;

    AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static MenuManager Instance { get; private set; }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}