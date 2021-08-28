using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
  public Transform lDoor, rDoor;



  void Update()
  {
    float openValue = Mathf.Clamp01((Time.time - 3) / 1);
    lDoor.localPosition = new Vector3(-1 - openValue * openValue, 1.5f, 2.25f);
    rDoor.localPosition = new Vector3(1 + openValue * openValue, 1.5f, 2.25f);
  }
}
