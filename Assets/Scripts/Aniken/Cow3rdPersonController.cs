using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow3rdPersonController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 5.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    //[Header("Player Grounded")]
    //[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    //public bool Grounded = true;
    //[Tooltip("Useful for rough ground")]
    //public float GroundedOffset = -0.14f;
    //[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    //public float GroundedRadius = 0.28f;
    //[Tooltip("What layers the character uses as ground")]
    //public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    private CowInputs _input;
    private Rigidbody _rb;
    private GameObject _mainCamera;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private const float _threshold = 0.01f;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<CowInputs>();
        _rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    //private void GroundedCheck()
    //{
    //    // set sphere position, with offset
    //    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
    //    Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    //}

    //private void OnDrawGizmos()
    //{
    //    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
    //    Gizmos.DrawSphere(spherePosition, GroundedRadius);
    //}

    private void Move()
    {
        Vector3 dir = transform.forward * moveSpeed * 100 * Time.deltaTime; ;
        _rb.velocity = new Vector3(dir.x, _rb.velocity.y, dir.z);
    }

    private void Rotate()
    {
        _targetRotation = _mainCamera.transform.eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

        // rotate to face input direction relative to camera position
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void Look()
    {
        if (_input.look.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetYaw += _input.look.x * Time.deltaTime;
            _cinemachineTargetPitch += _input.look.y * Time.deltaTime;
        }
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
