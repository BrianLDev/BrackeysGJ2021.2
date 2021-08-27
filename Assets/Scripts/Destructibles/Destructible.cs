using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EcxUtilities;
using RayFire;

public class Destructible : MonoBehaviour {
    [SerializeField] private DestructibleData data; // drag/drop corresponding data SO in Unity Editor

    RayfireRigid rigid;
    void Start() {
        RayfireRigid rigid = transform.GetComponent<RayfireRigid>();
        rigid.demolitionEvent.LocalEvent += DestroyMe;
    }

    void DestroyMe(RayfireRigid rigid) {
        ScoreManager.Instance.ChangeScore(data.pointsForDestroying);
        AudioManager.Instance.PlayClip(data.destroySfx, AudioCategory.Sfx, transform.position, 1, Random.Range(.8f, 1.2f));
    }

    void Stop() {
      rigid.demolitionEvent.LocalEvent -= DestroyMe;
    }
    // Add any more variables or methods here as needed
}
