using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float gravity = -9.81f;

    [Header("Animation")]
    public Animator animator;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            Debug.LogError("Animator is NOT assigned!");
        }
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD input
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed)
                x += 1f;

            if (Keyboard.current.aKey.isPressed)
                x -= 1f;

            if (Keyboard.current.wKey.isPressed)
                z += 1f;

            if (Keyboard.current.sKey.isPressed)
                z -= 1f;
        }

        // Calculate movement direction in world space
        Vector3 inputDirection = new Vector3(x, 0f, z).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Calculate the angle the character should face
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            
            // Smoothly rotate the character to face that angle
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Move player
        controller.Move(inputDirection * moveSpeed * Time.deltaTime);

        // Tell Animator how fast we're moving
        if (animator != null)
        {
            animator.SetFloat("Speed", inputDirection.magnitude);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}