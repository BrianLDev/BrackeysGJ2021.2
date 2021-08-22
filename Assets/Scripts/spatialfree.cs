using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spatialfree : MonoBehaviour
{
  [Header("References")]
  public float spread = 0.375f;
  public Transform leftFootMesh, rightFootMesh;
  Rigidbody rb;

  void Start()
  {
    rb = GetComponent<Rigidbody>();
  }

  void Update()
  {

  }

  float ramp;
  void FixedUpdate()
  {
    // rb.centerOfMass = com;
    // new Vector3((int)Mathf.Sin(Time.time * 3), 0, 0);
    // com = rb.centerOfMass;
    // rb.MoveRotation(Quaternion.Euler(0, Mathf.Sin(Time.time) * 45, 0));
    bool left, right;
    left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

    if (left && right)
    {
      rb.MovePosition(rb.position + rb.transform.forward * Time.fixedDeltaTime * ramp * 6);
      ramp = Mathf.Clamp01(ramp + Time.fixedDeltaTime * 5f);
    }
    else
    {
      if (left)
      {
        RotateRB(rb, Vector3.left * spread, Vector3.up, -Time.fixedDeltaTime * 260);
      }
      if (right)
      {
        RotateRB(rb, Vector3.right * spread, Vector3.up, Time.fixedDeltaTime * 260);
      }
      ramp = 0;
    }

    leftFootMesh.localPosition = new Vector3(-spread, right && !left ? 0.2f : 0, 0);
    rightFootMesh.localPosition = new Vector3(spread, left && !right ? 0.2f : 0, 0);
  }

  public void RotateRB(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
  {
    Quaternion q = Quaternion.AngleAxis(angle, axis);
    origin = rb.transform.TransformPoint(origin);
    rb.MovePosition(q * (rb.transform.position - origin) + origin);
    rb.MoveRotation(rb.transform.rotation * q);
  }
}
