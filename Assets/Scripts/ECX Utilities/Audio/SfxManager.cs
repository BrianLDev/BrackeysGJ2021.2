/*
ECX UTILITY SCRIPTS
Sfx Manager (Scriptable Object)
Last updated: June 18, 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EcxUtilities {
    /// <summary>
    /// This SfxManager is primarily used as a readily accessible SFX AudioClip storage.
    /// Other scripts can access audioclips as needed using SfxManager.xxx
    /// </summary>
    [CreateAssetMenu(fileName = "SfxManager-X", menuName = "ECX Utilities/SfxManager", order = 1)] 
    public class SfxManager : ScriptableObject {
        [Header("Destructible SFX")]   // UNCOMMENT THIS HEADER, RENAME IT, AND ADD ANY ADDITIONAL AUDIO CLIPS BELOW. THEN DRAG/DROP THEM IN THE UNITY EDITOR.
        public AudioClip[] boxDrops;
        public AudioClip[] glassBreaks;
        public AudioClip[] glassDrops;
        public AudioClip[] jarBreaks;
        public AudioClip[] woodBreaks;
        public AudioClip[] coins;
        public AudioClip cashRegister;
        public AudioClip allBroken;
        [Header("Bull SFX")]   // UNCOMMENT THIS HEADER, RENAME IT, AND ADD ANY ADDITIONAL AUDIO CLIPS BELOW. THEN DRAG/DROP THEM IN THE UNITY EDITOR.
        public AudioClip[] footsteps;
        public AudioClip[] bullSnorts;
        public AudioClip[] bullMoo;
        public AudioClip[] bullPain;

        // METHODS:
        // THE AUDIOMANAGER WILL HANDLE PLAYING MOST OF THE CLIPS, BUT IF YOU HAVE ANY CUSTOM SFX METHODS NEEDED, ADD THEM BELOW AS PUBLIC METHODS

    }
}