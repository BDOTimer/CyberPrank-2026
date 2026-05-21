using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float mouseSensitivityX = 0.2f;
    [SerializeField] private float mouseSensitivityY = 0.2f;
    [SerializeField] private float smoothing = 10.0f;
    [SerializeField] private float lookUpLimit = -90.0f;
    [SerializeField] private float lookDownLimit = 90.0f;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 10.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravity = 9.81f;

    private float targetRotationX = 0.0f;
    private float currentRotationX = 0.0f;
    private float targetRotationY = 0.0f;
    private float currentRotationY = 0.0f;
    private float rotationXVelocity = 0f;
    private float rotationYVelocity = 0f;

    private CharacterController controller;
    private Camera playerCamera;

    private Vector3 velocity = Vector3.zero;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private InputAction sprintAction;
    private InputAction quitAction;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private float verticalVelocity = 0.0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
        quitAction = playerInput.actions["Quit"];
        quitAction.performed += ctx => OnQuitPressed();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 startEuler = transform.localEulerAngles;
        Vector3 cameraEuler = playerCamera.transform.localEulerAngles;

        currentRotationY = NormalizeAngle(startEuler.y);
        targetRotationY = currentRotationY;

        currentRotationX = NormalizeAngle(cameraEuler.x);
        targetRotationX = currentRotationX;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
            jumpRequested = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
    }

    private void HandleCameraRotation()
    {
        targetRotationY += lookInput.x * mouseSensitivityX;
        targetRotationX -= lookInput.y * mouseSensitivityY;
        targetRotationX = Mathf.Clamp(targetRotationX, lookUpLimit, lookDownLimit);
        currentRotationY = Mathf.SmoothDamp(currentRotationY, targetRotationY, ref rotationYVelocity, 1f / smoothing);
        currentRotationX = Mathf.SmoothDamp(currentRotationX, targetRotationX, ref rotationXVelocity, 1f / smoothing);
        transform.localEulerAngles = new Vector3(0, currentRotationY, 0);
        playerCamera.transform.localEulerAngles = new Vector3(currentRotationX, 0, 0);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0) verticalVelocity = -2f;

        float speed = sprintAction.IsPressed() && isGrounded ? runSpeed : walkSpeed;
        Vector3 moveDirection = Vector3.zero;
        moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 horizontalVelocity = moveDirection.normalized * speed;

        if (jumpRequested && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            jumpRequested = false;
        }

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
        controller.Move(velocity * Time.deltaTime);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }

    private void OnQuitPressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
