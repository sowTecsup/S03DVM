using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputSystem_Actions inputs;
    private CharacterController controller;


    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;

    public bool IsDashing;
    public float DashForce = 10f;


    [SerializeField]private float verticalVelocity;
    public Vector2 moveInput;
    public bool isGrounded;
    private void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputs.Player.Sprint.performed += ctx => IsDashing = true;

        inputs.Player.Jump.performed += Jump;
    }


    private void OnDisable()
    {
        inputs.Disable();

    }
    void Start()
    {
        
    }

    
    void Update()
    {
        //MoveWithAumaticGrav();
        MoveWithSimulatedGrav();
    }
    public void MoveWithAumaticGrav()
    {
        //if (moveInput == Vector2.zero) return; 
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);
        Vector3 moveDir = transform.forward * moveInput.y * moveSpeed;
        controller.SimpleMove(moveDir);
    }
    public void MoveWithSimulatedGrav()
    {
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);
        Vector3 moveDir = transform.forward * moveInput.y * moveSpeed;
        isGrounded = controller.isGrounded;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        print(verticalVelocity);

        moveDir.y = verticalVelocity;


        if (IsDashing)
        {
            Vector3 dashDir = transform.forward;
            moveDir = transform.forward * DashForce;
            IsDashing = false;
        }
            


        controller.Move(moveDir * Time.deltaTime);

    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;

        verticalVelocity = jumpForce;
    }

}
