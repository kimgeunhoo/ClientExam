using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStats playerStats;

    [Header("Move")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float verticalVelocity = -0f;
    [SerializeField] private float pushPower = 8f;
    private float walkSpeed;
    private float runSpeed;
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float airControl = 0.4f;
    [SerializeField] private float landVelocityThreshold = -3f;

    [Header("Look")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform model;
    [SerializeField] private float turnSpeed = 12f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private CinemachineThirdPersonFollow thirdPersonFollow;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minCameraDistance = 2f;
    [SerializeField] private float maxCameraDistance = 8f;
    [SerializeField] private float zoomSmooth = 10f;

    private float zoomInput;
    private float targetCameraDistance;

    [Header("Animator")]
    [SerializeField] private Animator animator;


    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpStartHash = Animator.StringToHash("JumpStart");
    private static readonly int JumpLandHash = Animator.StringToHash("JumpLand");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");

    private CharacterController controller;
    private InventoryController inventoryController;
    private PlayerInputAction input;
    private PlayerRooting playerRooting;
    private PlayerMining playerMining;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool runPressed;
    private bool jumpPressed;

    private float yaw;
    private float pitch;

    private bool wasGrounded;


    private void Awake()
    {
        targetCameraDistance = thirdPersonFollow.CameraDistance;
        playerStats = GetComponent<CharacterStats>();
        walkSpeed = playerStats.MoveSpeed;
        runSpeed = playerStats.MoveSpeed * 2;
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (inventoryController == null)
            inventoryController = GetComponent<InventoryController>();
        if (playerRooting == null)
            playerRooting = GetComponentInChildren<PlayerRooting>();
        if (playerMining == null)
            playerMining= GetComponent<PlayerMining>();
        input = new PlayerInputAction();
    }
    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.Run.performed += OnRun;
        input.Player.Run.canceled += OnRun;
        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLook;
        input.Player.Zoom.performed += OnZoom;
        input.Player.Zoom.canceled += OnZoom;
        input.Player.Inventory.performed += inventoryController.OnInventory;
        input.Player.Inventory.canceled += inventoryController.OnInventory;
        input.Player.Pickup.performed += playerRooting.OnPickup;
        input.Player.Interact.performed += playerMining.OnInteract;

        input.Player.Jump.performed += OnJump; 
        input.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        Debug.Log($"input = {input}");
        Debug.Log($"inventoryController = {inventoryController}");
        Debug.Log($"playerRooting = {playerRooting}");
        Debug.Log($"playerMining = {playerMining}");
        if (input == null)
            return;

        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Player.Run.performed -= OnRun;
        input.Player.Run.canceled -= OnRun;
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLook;
        input.Player.Zoom.performed -= OnZoom;
        input.Player.Zoom.canceled -= OnZoom;
        input.Player.Inventory.performed -= inventoryController.OnInventory;
        input.Player.Inventory.canceled -= inventoryController.OnInventory;

        input.Player.Pickup.performed -= playerRooting.OnPickup;
        input.Player.Interact.performed -= playerMining.OnInteract;

        input.Player.Jump.performed -= OnJump;
        input.Player.Jump.canceled -= OnJump;
        input.Disable();
    }

    private void OnZoom(InputAction.CallbackContext ctx) 
    {
        zoomInput = ctx.ReadValue<float>();
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            playerMining.CancelMiningByInput();

        if (inventoryController != null && inventoryController.IsOpen)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = ctx.ReadValue<Vector2>();
    }
    private void OnRun(InputAction.CallbackContext ctx)
    {
        runPressed = ctx.ReadValueAsButton();
    }
    private void OnLook(InputAction.CallbackContext ctx)
    {
        if (inventoryController != null && inventoryController.IsOpen)
        {
            moveInput = Vector2.zero;
            return;
        }
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            playerMining.CancelMiningByInput();

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
        HandleCameraLook();
        HandleZoom();
        HandleMove();
        HandleJump();
        HandleAnimation();
    }
    private void HandleCameraLook()
    {
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -60f, 80f);

        cameraRoot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
    private void HandleZoom()
    {
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetCameraDistance -= zoomInput * zoomSpeed;

            targetCameraDistance = Mathf.Clamp(
                targetCameraDistance,
                minCameraDistance,
                maxCameraDistance
            );
        }

        thirdPersonFollow.CameraDistance = Mathf.Lerp(
            thirdPersonFollow.CameraDistance,
            targetCameraDistance,
            zoomSmooth * Time.deltaTime
        );

        zoomInput = 0f;
    }
    private void HandleAnimation()
    {
        float animSpeed = moveInput.magnitude;

        if (moveInput.magnitude > 0.1f)
        {
            animSpeed = runPressed ? 1f : 0.5f;
        }
        animator.SetFloat(MoveSpeedHash, animSpeed);
        animator.SetBool(IsGroundedHash, controller.isGrounded);
    }

    private void HandleMove()
    {

        Vector3 camForward = cameraTarget.forward;
        Vector3 camRight = cameraTarget.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            model.rotation = Quaternion.Slerp(
                model.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }

        float currentSpeed = runPressed ? runSpeed : walkSpeed;

        move *= currentSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void HandleJump() 
    {
        bool isGrounded = controller.isGrounded;
        float previousYVelocity = verticalVelocity;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (isGrounded && jumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            animator.ResetTrigger(JumpLandHash);
            animator.SetTrigger(JumpStartHash);
        }
        verticalVelocity += gravity * Time.deltaTime;

        bool isFalling = !isGrounded && verticalVelocity < -0.1f;
        animator.SetBool(IsFallingHash, isFalling);

        if (!wasGrounded && isGrounded && previousYVelocity <= landVelocityThreshold)
        {
            animator.SetBool(IsFallingHash, false);
            animator.SetTrigger(JumpLandHash);
        }

        wasGrounded = isGrounded;
        jumpPressed = false;
    }

}