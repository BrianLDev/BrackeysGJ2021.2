using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowTopDownController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 5.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    private CowInputs _input;
    private Rigidbody _rb;
    private GameObject _mainCamera;
    private Vector3 pointToLook;
    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<CowInputs>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateRotation();
        Move();
    }

    private void Move()
    {
        Vector3 dir = transform.forward * moveSpeed * 100 * Time.deltaTime; ;
        _rb.velocity = new Vector3(dir.x, _rb.velocity.y, dir.z);
    }

    private void CalculateRotation()
    {
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(_input.look);

        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return 180 - Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
