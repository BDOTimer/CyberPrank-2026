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

    [SerializeField] private float maxInteractDistance = 10f;
    [SerializeField] private LayerMask interactLayerMask;

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
    private InputAction attackAction;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    private float verticalVelocity = 0.0f;

    private IInteractable interactableObject;

    [HideInInspector] public bool IsKeys = true; /// +

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
        quitAction = playerInput.actions["Quit"];
        attackAction = playerInput.actions["Attack"];
        quitAction.performed += ctx => OnQuitPressed();
        attackAction.performed += ctx => OnAttackPerformed();
        attackAction.canceled += ctx => OnAttackCanceled();
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

        if(IsKeys) /// +
        {   HandleMovement();
        }
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

    private void OnAttackPerformed()
    {
        if (playerCamera == null) return;

        // Cast a ray forward from the camera
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Check if the ray hits an object on the interactable layer
        if (Physics.Raycast(ray, out hit, maxInteractDistance, interactLayerMask))
        {
            // If the object has an IInteractable component
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactableObject = interactable;
                interactable.OnInteract(hit.distance);
            }
        }

    }

    private void OnAttackCanceled()
    {
        if (interactableObject == null) return;

        interactableObject.OnInteractCanceled();
        interactableObject = null;
    }

}

public interface IInteractable
{
    void OnInteract(float hitDistance);
    void OnInteractCanceled();
}
