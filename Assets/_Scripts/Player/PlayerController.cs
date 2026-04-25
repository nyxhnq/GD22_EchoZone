using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchSprintSpeed = 4f;
    [SerializeField] private float jumpForce = 3.5f;
    [SerializeField] private float gravity = -12f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float standingCamY = 0.8f;
    [SerializeField] private float crouchingCamY = 0.4f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleCrouchToggle();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateCrouchTransition();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        float speed = walkSpeed;

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        if (isCrouching)
            speed = sprint ? crouchSprintSpeed : crouchSpeed;
        else
            speed = sprint ? sprintSpeed : walkSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // ƒвижение относительно направлени€ камеры
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // »гнорируем вертикальную составл€ющую направлени€
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * moveZ + right * moveX).normalized;
        controller.Move(move * speed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    private void HandleCrouchToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;
    }

    private void UpdateCrouchTransition()
    {
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        float targetCamY = isCrouching ? crouchingCamY : standingCamY;
        if (cameraTransform != null)
        {
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchTransitionSpeed * Time.deltaTime);
            cameraTransform.localPosition = camPos;
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}