using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using EcxUtilities;
using RayFire;

public class Destructible : MonoBehaviour
{
  [SerializeField] private DestructibleData data; // drag/drop corresponding data SO in Unity Editor
  private RayfireRigid rigid;
  private string itemName;

  void Start()
  {
    if (!data)
      Debug.LogError("Error: Missing destructible data on: " + this.itemName);
    RayfireRigid rigid = transform.GetComponent<RayfireRigid>();
    rigid.demolitionEvent.LocalEvent += DestroyMe;


    // destroyedItem = new KeyValuePair<int, int>(name, data.pointsForDestroying);
    itemName = rigid.gameObject.name;
    if (itemName.IndexOf('(') > -1)
    {
      itemName = itemName.Substring(0, itemName.IndexOf('('));
    }
    if (itemName.IndexOf('0') > -1)
    {
      itemName = itemName.Substring(0, itemName.IndexOf('0'));
    }
    itemName = itemName.Trim();
  }

  void DestroyMe(RayfireRigid rigid)
  {

    if (GameManager.Instance.destroyedItems.ContainsKey(itemName))
    {
      // then
      GameManager.Instance.destroyedItems[itemName] = new KeyValuePair<int, int>(data.pointsForDestroying, GameManager.Instance.destroyedItems[itemName].Value + 1);
    }
    else
    {
      GameManager.Instance.destroyedItems.Add(itemName, new KeyValuePair<int, int>(data.pointsForDestroying, 1));
    }
    Debug.Log(itemName);

    GameManager.Instance.score += data.pointsForDestroying;
    AudioManager.Instance.PlayClip(
        AudioManager.GetRandomClip(data.destroySfx),
        AudioCategory.Sfx,
        transform.position,
        1,
        Random.Range(.8f, 1.2f)
    );
  }

  void Stop()
  {
    rigid.demolitionEvent.LocalEvent -= DestroyMe;
  }
  // Add any more variables or methods here as needed
}
