﻿/*
ECX UTILITY SCRIPTS
Music Manager (Scriptable Object)
Last updated: June 18, 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcxUtilities {

    /// <summary>
    /// This MusicManager is primarily used as a readily accessible Music AudioClip storage.
    /// Other scripts can access audioclips as needed using MusicManager.Instance.xxx
    /// It also handles the automatic playback of music as soon as a specific Scene is loaded.
    /// </summary>
    [CreateAssetMenu(fileName = "MusicManager-X", menuName = "ECX Utilities/MusicManager", order = 1)] 
    public class MusicManager : ScriptableObject {

        [Header("Standard Music Tracks")]
        public AudioClip mainMenuMusic;
        public AudioClip gameMusic;
        public AudioClip gameOverMusic;

        // [Header("Game Specific Music Tracks")]   // UNCOMMENT THIS HEADER, RENAME IT, AND ADD ANY ADDITIONAL AUDIO CLIPS BELOW. THEN DRAG/DROP THEM IN THE UNITY EDITOR.

        
        // METHODS:
        private void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;  // subscribe to OnSceneLoaded event to play music for that scene
            // check to make sure Audioclips have been assigned
            if (!mainMenuMusic) { Debug.LogError("Error: missing AudioClip for mainMenuMusic"); }
            if (!gameMusic) { Debug.LogError("Error: missing AudioClip for gameMusic"); }
            if (!gameOverMusic) { Debug.LogError("Error: missing AudioClip for gameOverMusic"); }
        }

        // TODO: FIGURE OUT A BETTER WAY TO CONNECT MUSIC WITH RELATED SCENE.  BUILD INDEXES COULD CHANGE.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.buildIndex == 0)      // Main Menu
                AudioManager.Instance.PlayMusic(mainMenuMusic, 0.2f);
            else if (scene.buildIndex == 1) // Game
                AudioManager.Instance.PlayMusic(gameMusic, 0.2f);
            else if (scene.buildIndex == 2) // Game Over
                AudioManager.Instance.PlayMusic(gameOverMusic, 0.2f);
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;  // unsubscribe to OnSceneLoaded event to play music for that scene
        }
    }
}