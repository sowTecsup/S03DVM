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
    public float dashDuration = 0.2f;
    private float dashTimer;

    public float PushForce = 0.2f;

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

        inputs.Player.Sprint.performed += Dash;

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
            moveDir = transform.forward * DashForce * (dashTimer / dashDuration);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
                IsDashing = false;
        }



        controller.Move(moveDir * Time.deltaTime);

    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;

        verticalVelocity = jumpForce;
    }

    private void Dash(InputAction.CallbackContext context)
    {
        IsDashing = true;
        dashTimer = dashDuration;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        Vector3 pushDir =( hit.transform.position - transform.position).normalized;
        
        if (hit.rigidbody != null)
        {
            
            hit.rigidbody.AddForce(pushDir * PushForce, ForceMode.Impulse);
        }
    }

}
