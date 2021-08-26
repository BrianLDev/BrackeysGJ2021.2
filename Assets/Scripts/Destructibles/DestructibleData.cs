using System.Collections;
using System.Collections.Generic;
using EcxUtilities;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Destr-x", menuName = "ScriptableObjects/Destructible", order = 1)]
public class DestructibleData : ScriptableObject {
    [Tooltip("default = 1 kg. 1 lb = 0.45 kg")]
    public int mass;
    [Tooltip("approx. 25 points for small, 50 for medium, 100 for large, unique items have custom points")]
    public int pointsForDestroying;
    [Tooltip("Resistance to being destroyed. Default = 0.1 (glasslike)")]
    public float strengthVsDestruction;
    [Tooltip("The SFX clip played when object is destroyed")]
    public AudioClip destroySfx;
    // If we need to track any other destructible varialbes, add them here

    public void DestroyMe() {
        ScoreManager.Instance.ChangeScore(pointsForDestroying);
        AudioManager.Instance.PlayClip(destroySfx, AudioCategory.Sfx, 1, 1);    // TODO: add location of destroyed object if possible
    }
}
