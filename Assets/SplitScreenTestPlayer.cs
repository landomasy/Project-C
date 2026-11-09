/*
 * AI-Generated Code Citation
 * Model: Claude (Anthropic), Claude Sonnet 5.5
 * Date Generated: November 8, 2026
 * Prompt: "Write a small Unity test script using the new Input System's PlayerInput that moves
 *          and looks around for one player, handling mouse look and gamepad stick look differently,
 *          for testing two-player split-screen with keyboard and mouse plus a gamepad."
 */

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class SplitScreenTestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;          // Walking speed in meters per second
    public float mouseSensitivity = 0.1f; // Degrees of rotation per pixel of mouse movement
    public float stickLookSpeed = 150f;   // Degrees per second when the stick is fully pushed
    public float gravity = -20f;          // Pulls the player down (CharacterController has no gravity of its own)
    public Transform cameraPivot;         // The camera, which tilts up and down

    private PlayerInput playerInput;      // This player's own input (each player gets their own devices)
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction lookAction;
    private float pitch;                  // Current up/down camera angle
    private float verticalVelocity;       // Current falling speed

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        // Each Player Input has its own copy of the actions, so players don't affect each other
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    void Start()
    {
        // The mouse belongs to the keyboard player, so lock the cursor for them
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- Movement ---
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector3 direction = transform.right * move.x + transform.forward * move.y;

        // Keep the player stuck to the ground, and fall when not on it
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        controller.Move((direction * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        // --- Looking ---
        Vector2 look = lookAction.ReadValue<Vector2>();

        // A mouse reports how far it moved this frame, so it must NOT be multiplied by frame time.
        // A stick reports how far it is pushed, so it must be multiplied by frame time.
        bool usingGamepad = playerInput.currentControlScheme == "Gamepad";
        Vector2 delta = usingGamepad ? look * stickLookSpeed * Time.deltaTime : look * mouseSensitivity;

        transform.Rotate(Vector3.up * delta.x);                // Turn the body left and right
        pitch = Mathf.Clamp(pitch - delta.y, -80f, 80f);       // Tilt the camera, within limits
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}