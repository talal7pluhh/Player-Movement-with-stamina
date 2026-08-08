using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] Transform player_body;
    [SerializeField] PlayerInput pi;
    [SerializeField] bool invertY = false;
    [SerializeField] float maxPitch = 85f;
    [SerializeField] float look_sensitivity = 15f;
    Vector2 move_input;
    public InputActionReference look_action;
    float pitch = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        look_action.action.Enable();
    }
    private void OnDisable()
    {
        look_action.action.Disable();
    }
    private void Awake()
    {
        if (pi == null) 
            { pi = GetComponent<PlayerInput>(); }
    }

    private void Update()
    {
        if (look_action != null)
        {
            move_input = look_action.action.ReadValue<Vector2>();
        }
    }

    private void LateUpdate()
    {
        RotateCamera();
    }


    public void RotateCamera()
    {
        float mouseX = move_input.x * look_sensitivity * Time.deltaTime * look_sensitivity;
        float mouseY = move_input.y * look_sensitivity * Time.deltaTime * look_sensitivity;
        pitch += invertY ? mouseY : -mouseY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        if (player_body != null)
        {
            player_body.Rotate(Vector3.up * mouseX);
        }
    }
}
