using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Move")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float verticalVelocity = -0f;
    [SerializeField] private float pushPower = 8f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 4f; 

    [Header("Look")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    private CharacterController controller;
    private PlayerInputAction input;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool runPressed;
    private bool jumpPressed;
    private float yaw;
    private float pitch;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        input = new PlayerInputAction();
    }
    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLook;
        input.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLook;
        input.Player.Jump.performed -= OnJump;

        input.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic)
            return;

        // 아래 방향 충돌은 무시
        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

        rb.AddForceAtPosition(
            pushDir * pushPower,
            hit.point,
            ForceMode.Impulse
        );
    }
    private void Update()
    {
        HandleLook();
        HandleMove();
        HandleJump();
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        float animSpeed = moveInput.magnitude;

        if (runPressed && moveInput.magnitude > 0.1f)
            animSpeed = 1f;
        else if (moveInput.magnitude > 0.1f)
            animSpeed = 0.5f;

        animator.SetFloat(MoveSpeedHash, animSpeed);

        animator.SetBool(IsGroundedHash, controller.isGrounded);
    }

    private void HandleMove()
    {
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        float currentSpeed = runPressed ? runSpeed : walkSpeed;

        move *= currentSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60f, 70f);

        transform.rotation =
            Quaternion.Euler(0f, yaw, 0f);

        cameraRoot.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleJump() 
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger(JumpHash);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        jumpPressed = false;
    }
}