using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Настройки скорости")]
    public float walkSpeed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float crouchSprintSpeed = 2.5f;

    [Header("Настройки поворота")]
    public float rotationSpeed = 720.0f; // Насколько быстро персонаж поворачивается

    [Header("Физика")]
    public float gravity = 20.0f;

    [Header("Текущий статус (Для анимаций)")]
    public bool isSprinting;
    public bool isCrouching;
    public float currentSpeed;

    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        HandleInputs();
        MoveAndRotatePlayer();
    }

    void HandleInputs()
    {
        // Используем InputManager для управления
        isCrouching = InputManager.Instance != null && InputManager.Instance.IsCrouchHeld();
        isSprinting = InputManager.Instance != null && InputManager.Instance.IsSprintHeld();

        // Изменение размеров коллайдера при приседе
        if (isCrouching)
        {
            controller.height = originalHeight * 0.6f;
            controller.center = new Vector3(originalCenter.x, originalCenter.y * 0.6f, originalCenter.z);
        }
        else
        {
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
    }

    void MoveAndRotatePlayer()
    {
        // Получаем ввод с помощью InputManager
        Vector2 input = InputManager.Instance != null ? InputManager.Instance.GetMoveInput() : Vector2.zero;
        Vector3 movementInput = new Vector3(input.x, 0f, input.y);

        // Преобразуем движение относительно направления камеры
        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            movementInput = (camForward * input.y + camRight * input.x).normalized;
        }
        else
        {
            movementInput = movementInput.normalized;
        }

        // Выбираем скорость
        if (isCrouching)
            currentSpeed = isSprinting ? crouchSprintSpeed : crouchSpeed;
        else
            currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 velocity = movementInput * currentSpeed;

        if (controller.isGrounded)
        {
            // Сброс вертикальной скорости при касании земли
            if (moveDirection.y < 0f)
                moveDirection.y = -1f;

            // Поворот в сторону движения
            if (movementInput.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementInput);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Применяем гравитацию
        moveDirection.y += -gravity * Time.deltaTime;

        // Итоговое движение
        Vector3 finalMove = velocity;
        finalMove.y = moveDirection.y;

        controller.Move(finalMove * Time.deltaTime);

        // Обновляем вертикальную скорость для следующего кадра
        if (controller.isGrounded && moveDirection.y < 0f)
            moveDirection.y = -1f;
    }
}