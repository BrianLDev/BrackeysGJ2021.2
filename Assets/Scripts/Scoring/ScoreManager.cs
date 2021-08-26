using System.Collections;
using System.Collections.Generic;
using EcxUtilities;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : SingletonScoring<ScoreManager> {
    public int score { get; private set; }
    [SerializeField] private string scoreUiText;    // feel free to change this if needed depending on UI
    // any other UI stuff here

    private void Awake() {
        ResetScore();
    }

    public void ChangeScore(int amount) {
        score += amount;
    }

    public void ResetScore() {
        score = 0;
    }
}
