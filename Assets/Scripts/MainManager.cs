using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    [SerializeField] TextMeshProUGUI roundCountText;
    [SerializeField] TextMeshProUGUI smallRoundCountText;
    [SerializeField] AudioClip newRoundClip;

    public Text ScoreText;
    public Text bestScore;
    public GameObject GameOverText;
    [SerializeField] AudioClip gameOverClip;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    public int roundCount = 0;

    void Start()
    {
        BestScoreUpdate();
        AddPoint(0);
        roundCount = 1;

        SpawnRound();
        roundCountText.text = null;
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2f, ForceMode.VelocityChange);
            }
        }
        else if (FindObjectsByType<Brick>(FindObjectsSortMode.None).Length == 0)
        {
            roundCount++;
            SpawnRound();
            PlaySound(newRoundClip);
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    public void PlaySound(AudioClip clip)
    {
        MenuManager.Instance.PlaySound(clip);
    }

    void SpawnRound()
    {
        StartCoroutine(ShowRoundCount());
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    void AddPoint(int point)
    {
        m_Points += point;
        if (MenuManager.Instance != null)
        {
            ScoreText.text = $"{MenuManager.Instance.playerName} : {m_Points}";
        }
        else
        {
            ScoreText.text = $"Name : {m_Points}";
        }
    }

    IEnumerator ShowRoundCount()
    {
        smallRoundCountText.text = "Round:" + roundCount;
        roundCountText.text = "Round:" + roundCount;
        yield return new WaitForSeconds(1);
        roundCountText.text = null;
    }
    public void GameOver()
    {
        m_GameOver = true;
        PlaySound(gameOverClip);

        if (MenuManager.Instance != null)
        {
            SaveSystem.UpdateHighscoreList(SaveSystem.highscoreSave, MenuManager.Instance.playerName, m_Points);
            MenuManager.Instance.score = m_Points;
        }
        SceneManager.LoadScene(2);
    }

    private void BestScoreUpdate()
    {
        if (SaveSystem.DisplayHighscores(SaveSystem.highscoreSave) != null)
        {
            List<HighscoreEntry> highscores = SaveSystem.DisplayHighscores(SaveSystem.highscoreSave);
            bestScore.text = "Best Score: " + highscores[0].playerName + " " + highscores[0].score;
        }
    }
}
