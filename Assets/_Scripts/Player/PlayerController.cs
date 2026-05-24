using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float turnSpeed = 90f;

    private bool isRunning = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        Vector2 input = InputManager.Instance != null ? InputManager.Instance.GetMoveInput() : Vector2.zero;
        isRunning = InputManager.Instance != null && InputManager.Instance.IsSprintHeld();

        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        if (moveDir != Vector3.zero)
        {
            // Поворот в сторону движения
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

        controller.Move(moveDir * speed * Time.deltaTime - Vector3.up * 0.1f);
    }

    void HandleAnimation()
    {
        Vector2 input = InputManager.Instance != null ? InputManager.Instance.GetMoveInput() : Vector2.zero;
        bool isMoving = input.magnitude > 0.1f;
        isRunning = InputManager.Instance != null && InputManager.Instance.IsSprintHeld();

        if (!isMoving)
        {
            animator.Play("Doctor_01|Idle_03");
        }
        else if (isMoving && !isRunning)
        {
            animator.Play("Doctor_01|Walk_01");
        }
        else if (isMoving && isRunning)
        {
            animator.Play("Doctor_01|Running_01");
        }
    }
}