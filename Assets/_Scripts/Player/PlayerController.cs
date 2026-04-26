using UnityEngine;

/// <summary>
/// Player movement controller for 3D third-person.
/// Reads input from InputManager, moves a CharacterController
/// relative to the camera and rotates the visual model in a
/// "strafing" style (character always looks where the camera looks).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player stats component (health, move speed, jump force, etc.).")]
    [SerializeField] private PlayerStats playerStats;

    [Tooltip("Camera transform used as reference for movement (usually main camera or Cinemachine virtual camera).")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Root transform of the visual model (rotates to face camera).")]
    [SerializeField] private Transform visualRoot;

    [Header("Movement & Physics")]
    [Tooltip("Gravity value (negative).")]
    [SerializeField] private float gravity = -9.81f;

    [Tooltip("Small downward velocity to keep the character grounded.")]
    [SerializeField] private float groundedGravity = -2f;

    [Tooltip("Speed multiplier when sprinting.")]
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Crouch")]
    [Tooltip("Множитель скорости при приседании.")]
    [SerializeField] private float crouchMultiplier = 0.5f;
    [Tooltip("Высота коллайдера при приседании.")]
    [SerializeField] private float crouchHeight = 1.0f;
    [Tooltip("Высота коллайдера в обычном состоянии.")]
    [SerializeField] private float standHeight = 2.0f;
    [Tooltip("Скорость перехода между высотами.")]
    [SerializeField] private float crouchTransitionSpeed = 8f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;
    private bool isGrounded;

    // Состояние приседания
    private bool isCrouching = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (InputManager.Instance == null)
            return;

        if (playerStats != null && playerStats.IsDead)
        {
            InputManager.Instance.ResetButtonFlags();
            return;
        }

        HandleCrouchToggle();
        HandleMovement();
        HandleJump();
        UpdateCrouchTransition();

        InputManager.Instance.ResetButtonFlags();
    }

    /// <summary>
    /// Обработка переключения состояния приседания.
    /// </summary>
    private void HandleCrouchToggle()
    {
        if (InputManager.Instance.IsCrouchHeld())
        {
            isCrouching = !isCrouching;
        }
    }

    /// <summary>
    /// Считает движение относительно камеры, применяет гравитацию
    /// и двигает CharacterController. Также обновляет поворот визуальной
    /// модели так, чтобы персонаж всегда смотрел туда же, куда и камера.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;
        Vector3 moveDirection = Vector3.zero;

        if (moveInput.sqrMagnitude > 0.001f && cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();

            moveDirection = forward * moveInput.y + right * moveInput.x;
            moveDirection.Normalize();
        }

        float speed = 5f;
        float rotationSpeed = 720f;

        if (playerStats != null && playerStats.playerData != null)
        {
            speed = playerStats.playerData.moveSpeed;
            rotationSpeed = playerStats.playerData.rotationSpeed;
        }

        if (InputManager.Instance.IsSprintHeld())
        {
            speed *= sprintMultiplier;
        }

        // Замедление при приседании
        if (isCrouching)
        {
            speed *= crouchMultiplier;
        }

        Vector3 horizontalVelocity = moveDirection * speed;

        isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedGravity;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);

        if (cameraTransform != null && visualRoot != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                visualRoot.rotation = Quaternion.Slerp(
                    visualRoot.rotation,
                    targetRotation,
                    rotationSpeed * Mathf.Deg2Rad * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// Плавное изменение высоты коллайдера при приседании.
    /// </summary>
    private void UpdateCrouchTransition()
    {
        if (characterController == null)
            return;

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Обрабатывает прыжок: если игрок стоит на земле и кнопка прыжка
    /// была нажата в этом кадре, задаёт вертикальную скорость вверх.
    /// </summary>
    private void HandleJump()
    {
        if (!isGrounded)
            return;

        if (InputManager.Instance.IsJumpPressed())
        {
            float jumpForce = 5f;

            if (playerStats != null && playerStats.playerData != null)
            {
                jumpForce = playerStats.playerData.jumpForce;
            }

            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
}