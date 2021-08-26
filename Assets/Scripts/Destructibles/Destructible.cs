using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EcxUtilities;

public class Destructible : MonoBehaviour {
    [SerializeField] private DestructibleData data; // drag/drop corresponding data SO in Unity Editor

    // TODO: CALL DESTROYME AT INSTANT WHEN RAYFIRE DESTRUCTION IS TRIGGERED (WHICH IS BEFORE GAMEOBJECT DESTROYED)
    public void DestroyMe() {
        ScoreManager.Instance.ChangeScore(data.pointsForDestroying);
        AudioManager.Instance.PlayClip(data.destroySfx, AudioCategory.Sfx, transform.position, 1, Random.Range(.8f, 1.2f));
    }

    // Add any more variables or methods here as needed
}
