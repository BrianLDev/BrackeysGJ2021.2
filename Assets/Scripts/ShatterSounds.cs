using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class ShatterSounds : MonoBehaviour
{
  const int numSrcs = 12;
  List<AudioSource> srcs = new List<AudioSource>();
  public AudioClip[] defaultSounds;

  void Start()
  {
    RFDemolitionEvent.GlobalEvent += GlobalEvented;

    // audio src pool
    for (int i = 0; i < numSrcs; i++)
    {
      GameObject go = new GameObject("ShatterSound " + i);
      AudioSource src = go.AddComponent<AudioSource>();
      srcs.Add(src);
    }
  }

  void GlobalEvented(RayfireRigid rigid)
  {
    // play sound
    for (int i = 0; i < srcs.Count; i++)
    {
      AudioSource src = srcs[i];
      if (!src.isPlaying)
      {
        src.transform.position = rigid.physics.position;
        // override with an audio clip attached to the prefab else:
        src.clip = defaultSounds[Random.Range(0, defaultSounds.Length)]; 
        src.volume = 0.5f + Random.value;
        src.Play();
        return;
      }
    }

    Debug.LogWarning("Maxed Out Shatter Audio Sources");
  }
}