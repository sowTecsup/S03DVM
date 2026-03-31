using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputSystem_Actions inputs;
    private CharacterController controller;



    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    public float verticalVelocity = 0;
    public float jumpForce = 10;

    public float pushForce = 4;

    [SerializeField]private Vector2 moveInput;

    private void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Move.performed += ctx =>  moveInput = ctx.ReadValue<Vector2>();
        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;


        inputs.Player.Jump.performed += OnJump;

        

    }
    void Start()
    {

    }

    
    void Update()
    {

         OnMove();
        //OnSimpleMove();
    }

    public void OnMove()
    {
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);
        Vector3 moveDir = transform.forward * moveSpeed * moveInput.y;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if(controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;


        moveDir.y = verticalVelocity;
        //print(moveDir);
        controller.Move(moveDir * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;

        verticalVelocity = jumpForce;
    }
    public void OnSimpleMove()
    {
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);
        Vector3 moveDir = transform.forward * moveSpeed * moveInput.y ;
        controller.SimpleMove(moveDir);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        print(hit.gameObject.name);

        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null)
            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }

}
