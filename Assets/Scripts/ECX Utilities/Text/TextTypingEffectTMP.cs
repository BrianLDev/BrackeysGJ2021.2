/*
ECX UTILITY SCRIPTS
Text Typing Effect - TextMesh Pro (TMP)
Last updated: Mar 20, 2021
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace EcxUtilities
{
    /// <summary>
    /// Creates a typing effect for TextMesh Pro text fields that reveals letters over time.
    /// </summary>
    public class TextTypingEffectTMP : MonoBehaviour
    {

        public enum TypingSpeedTypes { Instant, Blazing, VeryFast, Fast, Normal, Slow, VerySlow, OcarinaOfTime }
        public Dictionary<TypingSpeedTypes, float> typingSpeedDict = new Dictionary <TypingSpeedTypes, float> {
            {TypingSpeedTypes.Instant, 0f}, // instant will display the full text block all at once. All others reveal over time.
            {TypingSpeedTypes.Blazing, 0f}, {TypingSpeedTypes.VeryFast, 0.01f}, {TypingSpeedTypes.Fast, 0.02f}, 
            {TypingSpeedTypes.Normal, 0.04f}, 
            {TypingSpeedTypes.Slow, 0.06f}, {TypingSpeedTypes.VerySlow, 0.1f}, {TypingSpeedTypes.OcarinaOfTime, 0.2f} 
        };
        public TypingSpeedTypes typingSpeed = TypingSpeedTypes.Normal;
        public bool beginOnStart = true;    // starts revealing the typing on Start().  If set to false, use BeginTypingEffect() to manually initiate typing effect
        public bool revealWordsNotChars = false;   // default to reveal characters, but can reveal words if this is set to true

        private TMP_Text TmpTextField;
        private int visibleCount = 0;   // tracks how many characters/words have been made visible so far
        private float waitTime;
        private bool hasTextChanged;
        private bool isTyping = false; 
        public bool IsTyping => isTyping;   // public accessor to check if still typing or done
        private bool instantReveal = false; // when set to true, all characters are revealed instantly


        public void Awake() {
            TmpTextField = gameObject.GetComponent<TMP_Text>();
            TmpTextField.maxVisibleCharacters = 0;  // make all text invisible to start with
        }

        public void Start() {
            if (beginOnStart)
                BeginTypingEffect();
        }

        public void OnEnable() {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED); // Subscribe to event fired when text object has been regenerated.
        }

        public void OnDisable() {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);  // Unsubscribe to event fired when text object has been regenerated.
        }

        public void BeginTypingEffect() {
            isTyping = true;
            this.gameObject.SetActive(true);
            TmpTextField.maxVisibleCharacters = 0;  // make all text invisible to start with
            if (revealWordsNotChars)
                StartCoroutine(RevealWords(TmpTextField));
            else
                StartCoroutine(RevealCharacters(TmpTextField));
        }

        public void ClearVisibleText() {
            visibleCount = 0;
        }

        public void SetTextField(string newText) {
            TmpTextField.text = newText;
        }

        public string GetTextField() {
            return TmpTextField.text;
        }

        public void InstantRevealAllChars() {
            instantReveal = true;
        }

        // Event received when the text object has changed.
        void ON_TEXT_CHANGED(Object obj) {
            hasTextChanged = true;
        }

        /// <summary>
        /// Method revealing the text one character at a time.
        /// </summary>
        /// <returns></returns>
        IEnumerator RevealCharacters(TMP_Text textComponent) {
            textComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = textComponent.textInfo;
            int totalCharacters = textInfo.characterCount; // Get # of characters in text object
            visibleCount = 0;

            while(isTyping) {   // Coroutine types each character in this loop
                if (hasTextChanged) {
                    totalCharacters = textInfo.characterCount; // Update visible character count.
                    hasTextChanged = false; 
                    waitTime = typingSpeedDict[typingSpeed];
                    yield return new WaitForSeconds( waitTime );   // add a small delay between each character typed
                }

                if (typingSpeed == TypingSpeedTypes.Instant || instantReveal == true) {
                    visibleCount = totalCharacters+1;   // Reveal all characters
                }

                textComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

                if (visibleCount > totalCharacters) {    // reached end of text. Set bools to false so coroutine can exit loop
                    isTyping = false;
                    instantReveal = false;
                }
                
                visibleCount += 1;
                yield return null;
            }
            yield return null;
        }

        /// <summary>
        /// Method revealing the text one word at a time.
        /// </summary>
        /// <returns></returns>
        IEnumerator RevealWords(TMP_Text textComponent) {
            textComponent.ForceMeshUpdate();
            int totalWordCount = textComponent.textInfo.wordCount;
            int totalCharacters = textComponent.textInfo.characterCount; // Get # of characters in text object
            int counter = 0;
            int currentWord = 0;
            visibleCount = 0;

            while (isTyping) {
                currentWord = counter % (totalWordCount + 1);

                // Get last character index for the current word.
                if (currentWord == 0) // Display no words.
                    visibleCount = 0;
                else if (currentWord < totalWordCount) // Display all other words with the exception of the last one.
                    visibleCount = textComponent.textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
                else if (currentWord == totalWordCount) // Display last word and all remaining characters.
                    visibleCount = totalCharacters;

                if (typingSpeed == TypingSpeedTypes.Instant || instantReveal == true) {
                    visibleCount = totalCharacters+1;   // Reveal all characters
                }

                textComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

                // Once the last character has been revealed
                if (visibleCount >= totalCharacters) {      // reached end of text. Set bools to false so coroutine can exit loop
                    isTyping = false;
                    instantReveal = false;
                }

                counter += 1;

                waitTime = typingSpeedDict[typingSpeed];
                waitTime *= 8.5f;  // multiply by 8.5 to match character waitTime for similar amount of text
                yield return new WaitForSeconds( waitTime );   // add a small delay between each character typed
            }
            yield return null;
        }

    }
}