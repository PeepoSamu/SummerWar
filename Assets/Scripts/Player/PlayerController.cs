using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float upForce = 5f;
    [SerializeField] Transform cameraPivot;
    [SerializeField] float sensitivity = 100f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        //Movimiento relativo al player (no mundo)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 movement = forward * moveInput.y + right * moveInput.x;
        movement = movement.normalized;

        rb.AddForce(movement * speed, ForceMode.Force);
    }

    private void LateUpdate()
    {
        Rotating();
    }

    void Rotating()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        //Vertical (cámara)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Horizontal (player) usando física
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));
    }

    public void Looking(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Moving(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jumping(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        }
    }
}