using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EcxUtilities;

public class GameManager : SingletonGameManager<GameManager> {
    // ADD CODE HERE!


    public GameplayUIManager gameplayUI;

    public GameOverUIManager gameOverUI;

    public int score;

    public bool gameOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        gameOver = false;
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
            yield return new WaitForSecondsRealtime(0.2f);
        }
        int amount = 0;
        while (amount <= score)
        {
            amount += 300;
            gameOverUI.updateTotal(amount);
            yield return null;
        }
        amount = score;
        gameOverUI.updateTotal(amount);

    }

}