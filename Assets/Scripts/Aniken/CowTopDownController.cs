using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowTopDownController : MonoBehaviour
{
    public enum PlayerState { MOVEMENT, DASH }

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 5.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 5f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to dash again. Set to 0f to instantly dash again")]
    public float DashTimeout = 3.0f;

    [Header("PauseUI")]
    public GameObject PauseUI;

    private CowInputs _input;
    private Rigidbody _rb;
    private Animator _anim;
    private GameObject _mainCamera;
    private Vector3 pointToLook;
    private PlayerState state;

    private float _dashTimeoutDelta;
    private float _rotationVelocity;
    private float _dashCount = 3;
    private bool _allDashes;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<CowInputs>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _allDashes = true;
        Time.timeScale = 0;
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateRotation();
        Dash();
        Move();
        Pause();
    }

    private void Move()
    {
        Vector3 dir = transform.forward * moveSpeed * 100 * Time.deltaTime; ;
        _rb.velocity = new Vector3(dir.x, _rb.velocity.y, dir.z);
    }

    private void CalculateRotation()
    {
        if (state == PlayerState.MOVEMENT)
        {
            Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

            Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(_input.look);

            float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref _rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
    }

    private void Dash()
    {
        if (_input.dash && _dashCount > 0 && !GameManager.Instance.pause &&state == PlayerState.MOVEMENT)
        {
            state = PlayerState.DASH;
            --_dashCount;
            GameManager.Instance.gameplayUI.removeDash();
            StartCoroutine(DashCoroutine());

        }

        if (_dashTimeoutDelta >= 0.0f)
        {
            _dashTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            if(!_allDashes)
            {
                ++_dashCount;
                GameManager.Instance.gameplayUI.RestoreDash();

            }
            if (_dashCount < 3)
            {
                _dashTimeoutDelta = DashTimeout;
                GameManager.Instance.gameplayUI.clockAnim.SetTrigger("CoutDown");
                _allDashes = false;
            }
            else
            {
                _allDashes = true;
            }
        }
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        moveSpeed *= 2;
        _anim.speed = 2;
        while (Time.time < startTime + 1f)
        {
            yield return null;
        }
        moveSpeed /= 2;
        _anim.speed = 1;
        state = PlayerState.MOVEMENT;
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return 180 - Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void Pause()
    {
        if (_input.pause)
        {
            if (!GameManager.Instance.pause)
            {
                Time.timeScale = 0;
                PauseUI.SetActive(true);
                GameManager.Instance.pause = true;
            }
            else
            {
                Time.timeScale = 1;
                PauseUI.SetActive(false);
                GameManager.Instance.pause = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(state == PlayerState.DASH)
        {
            if(collision.gameObject.CompareTag("Wall"))
            {
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + 180, 0f);
            }
        }
    }
}
