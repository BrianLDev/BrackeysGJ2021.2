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
        foreach(var item in GameManager.Instance.destroyedItems)
        {
            GameObject destroyed = Instantiate(ItemTemplate, ItemContent);

            destroyed.transform.GetChild(0).GetComponent<Text>().text = item.Key;
            destroyed.transform.GetChild(1).GetComponent<Text>().text = item.Value.Value.ToString();
            destroyed.transform.GetChild(2).GetComponent<Text>().text = "$" + item.Value.Key.ToString("##,#");
            destroyed.transform.GetChild(3).GetComponent<Text>().text = "$" +(item.Value.Key * item.Value.Value).ToString("##,#");
            destroyed.SetActive(false);
        }
    }

    public void updateTotal(int amount)
    {
        total.text = "$" + amount.ToString("##,#");
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
