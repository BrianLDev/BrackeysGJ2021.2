using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{

    public GameObject ItemTemplate;

    public Transform ItemContent;

    public Text total;

    public Button restart;

    void Awake()
    {
        GameManager.Instance.gameOverUI = this;
        this.gameObject.SetActive(false);
        GameManager.Instance.gameOver = false;
    }

    public void AddItemsInContent()
    {
        for (int i = 0; i < 15; ++i)
        {
            GameObject item = Instantiate(ItemTemplate, ItemContent);
            item.SetActive(false);
        }
    }

    public void updateTotal(int amount)
    {
        total.text = amount.ToString("##,#");
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
