using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Timer")]
    [Tooltip("Text Mesh Pro Object for Timer")]
    public TextMeshProUGUI timerText;
    [Tooltip("Gameplay Time in seconds")]
    public float gameTime;

    [Space(10)]
    [Header("Score")]
    [Tooltip("Text Mesh Pro Object for Score")]
    public TextMeshProUGUI scoreText;

    [Space(10)]
    [Header("Others")]
    [Tooltip("The Clock counter for the dash timeout")]
    public Animator clockAnim;
    [Tooltip("GameObject hold the number of Dashes")]
    public Transform dashCounts;
    public Queue<int> dashIndexs;

    private float _timer;
    private float _currentScore;

    void Awake()
    {
        GameManager.Instance.gameplayUI = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _timer = gameTime;
        dashIndexs = new Queue<int>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
        GameManager.Instance.score += 1;
        setScore(GameManager.Instance.score);
        if(_currentScore < GameManager.Instance.score)
        {
            _currentScore += 10;
            SetScoreText();
        }
        else
        {
            _currentScore = GameManager.Instance.score;
        }

    }

    public void Timer()
    {
        int minutes = Mathf.FloorToInt(_timer / 60F);
        int seconds = Mathf.FloorToInt(_timer % 60F);
        if (_timer >= 0.0f)
        {
            _timer -= Time.deltaTime;
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            timerText.text = "00:00";
            if (!GameManager.Instance.gameOver)
            {
                _currentScore = GameManager.Instance.score;
                GameManager.Instance.GameOver();
            }
        }
        
    }

    public void SetScoreText()
    {
        scoreText.text = "$ " + _currentScore.ToString("##,#");
    }

    public void removeDash()
    {
        for (int i = 0; i < dashCounts.childCount; ++i)
        {
            if(dashCounts.GetChild(i).gameObject.activeSelf)
            {
                dashCounts.GetChild(i).gameObject.SetActive(false);
                dashIndexs.Enqueue(i);
                return;
            }
        }
    }

    public void RestoreDash()
    {
        if(dashIndexs.Count > 0)
        {
            dashCounts.GetChild(dashIndexs.Dequeue()).gameObject.SetActive(true);
        }
    }
}
