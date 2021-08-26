using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CowInputs : MonoBehaviour
{

    public Vector2 look { get; private set;}
    public Vector2 rotate { get; private set;}

    public void OnRotate(InputValue value)
    {
        rotate = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }
}
