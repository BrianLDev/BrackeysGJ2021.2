using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EcxUtilities;

public class GameManager : SingletonGameManager<GameManager> {
    // ADD CODE HERE!


    public GameplayUIManager gameplayUI;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void GameOver()
    {
    }

}