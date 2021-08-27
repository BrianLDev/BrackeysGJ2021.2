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
    private int _score;

    [Space(10)]
    [Header("Others")]
    [Tooltip("The Clock counter for the dash timeout")]
    public Animator clockAnim;
    [Tooltip("GameObject hold the number of Dashes")]
    public Transform dashCounts;
    public Queue<int> dashIndexs;

    private float _timer;

    void Awake()
    {
        GameManager.Instance.gameplayUI = this;
        _score = 10000;
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
        _score += 1;
        setScore(_score);

    }

    public void Timer()
    {
        if(_timer >= 0.0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            GameManager.Instance.GameOver();
        }
        int minutes = Mathf.FloorToInt(_timer / 60F);
        int seconds = Mathf.FloorToInt(_timer % 60F);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void setScore(int score)
    {
        _score = score;
        scoreText.text ="$ " + _score.ToString("##,#");
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
