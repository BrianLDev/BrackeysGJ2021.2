using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EcxUtilities;

public class GameManager : SingletonGameManager<GameManager> {
    // ADD CODE HERE!


    public GameplayUIManager gameplayUI;

    public GameOverUIManager gameOverUI;

    public int score;

    public int highScore;

    [Tooltip("Gameplay Time in seconds")]
    public float gameTime = 180;

    public Dictionary<string, KeyValuePair<int, int>> destroyedItems;

    public bool pause;

    public bool gameOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        string path = Application.persistentDataPath + "/score.dat";
        if (File.Exists(path))
        {
            PlayerScore thisData = SaveScore.Load();
            highScore = thisData.score;
        }
        destroyedItems = new Dictionary<string, KeyValuePair<int, int>>();
    }

    private void Start()
    {
        gameOver = false;
        pause = true;
        score = 0;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOver = true;
        //pause = true;
        StartCoroutine(gameOverCoroutin());

    }

    private IEnumerator gameOverCoroutin()
    {
        gameOverUI.AddItemsInContent();
        gameOverUI.updateHighScore(highScore);
        yield return new WaitForSecondsRealtime(1.0f);
        gameOverUI.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        Transform items = gameOverUI.ItemContent;
        for (int i = 0; i < items.childCount; ++i)
        {
            items.GetChild(i).gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            gameOverUI.Scroll.verticalNormalizedPosition = 0f;
            yield return new WaitForSecondsRealtime(0.2f);
        }
        int amount = 0;
        while (amount < score)
        {
            amount += 100;
            gameOverUI.updateTotal(amount);
            yield return null;
        }
        gameOverUI.updateTotal(amount);
        if (score > highScore)
        {
            amount = highScore;
            highScore = score;
            SaveScore.Save();

            while (amount < highScore)
            {
                amount += 100;
                gameOverUI.updateHighScore(amount);
                yield return null;
            }
        }
        gameOverUI.restart.SetActive(true);

    }

    public void RestartGame()
    {
        score = 0;
        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        gameOverUI.transition.SetTrigger("Enter");
        yield return new WaitForSecondsRealtime(2.5f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while(!operation.isDone)
        {
            yield return null;
        }
    }
}