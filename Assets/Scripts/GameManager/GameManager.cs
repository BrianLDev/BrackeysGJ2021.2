﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EcxUtilities;

public class GameManager : SingletonGameManager<GameManager> {
    // ADD CODE HERE!


    public GameplayUIManager gameplayUI;

    public GameOverUIManager gameOverUI;

    public int score;

    [Tooltip("Gameplay Time in seconds")]
    public float gameTime = 180;

    public Dictionary<string, KeyValuePair<int, int>> destroyedItems;

    public bool gameOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        destroyedItems = new Dictionary<string, KeyValuePair<int, int>>();
    }

    private void Start()
    {
        gameOver = false;
        score = 0;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOver = true;
        StartCoroutine(gameOverCoroutin());
    }

    private IEnumerator gameOverCoroutin()
    {
        gameOverUI.AddItemsInContent();
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
        while (amount <= score)
        {
            amount += 100;
            gameOverUI.updateTotal(amount);
            yield return null;
        }
        amount = score;
        gameOverUI.updateTotal(amount);
        gameOverUI.restart.gameObject.SetActive(true);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        score = 0;
    }

}