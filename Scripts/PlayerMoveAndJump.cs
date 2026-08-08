using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMoveAndJump : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float rotation_speed = 10f;
    public float _speed = 5f;
    [SerializeField] private float acceleration_speed = 12f;
    [SerializeField] private float crouch_speed = 7f; 
    [SerializeField] private float GRAVITY = -20f;
    [SerializeField] private float jump_height = 1.5f;
    // [SerializeField] private Vector3 player_scale_target = new Vector3(1f, 0.5f, 1f);
    public CharacterController _controller;
    public Vector3 player_velocity;
    private Vector2 input_;
    // private Vector2 smooth_input_velocity;
    private bool is_grounded;
    [SerializeField] private Transform cam_;
    private float extra_fall_multiplier = 0.82f;
    private Vector3 player_standing_scale = new Vector3(1f, 1f, 1f);
    private Vector3 player_crouching_scale = new Vector3(1f, 0.5f, 1f);

    private float transition_speed = 12f;
    public float standing_position_cameraY = 1.2f;
    public float camera_crouch_positionY = 0.32f;
    // private float target_camera_y;

    [Header("Input Actions")]
    public InputActionReference _move_action;
    public InputActionReference _jump_action;
    public InputActionReference _sprint_action;
    public InputActionReference _crouch_action;

    // [Header("Triggers")]
    // [SerializeField] GameObject head_trigger_collider;
    private StaminaBar stamina_;
    private void Start()
    {
        stamina_ = GetComponent<StaminaBar>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Camera.main != null)
        {
            cam_ = Camera.main.transform;
        }
        transform.localScale = Vector3.one;
    }

    public void RunSpeed(float speed)
    {
        speed = acceleration_speed;
    }

    private void OnEnable()
    {
        _move_action.action.Enable();
        _jump_action.action.Enable();
        _sprint_action.action.Enable();
        _crouch_action.action.Enable();
    }

    private void OnDisable()
    {
        _move_action.action.Disable();
        _jump_action.action.Disable();
        _sprint_action.action.Disable();
        _crouch_action.action.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        is_grounded = _controller.isGrounded;
        if (is_grounded)
        {
            // keep the player in the ground
            if (player_velocity.y < -2f)
            {
                player_velocity.y = -2.8f;
            }
        }
        else
        {
            player_velocity.y += Physics.gravity.y * extra_fall_multiplier * Time.deltaTime; 
        }
            // reading input every second //
        Vector2 input = _move_action.action.ReadValue<Vector2>();
        input_ = Vector2.Lerp(input_, input, 1f - Mathf.Exp(-acceleration_speed * Time.deltaTime * rotation_speed));
        Vector3 camForward = cam_.forward;
        Vector3 camRight = cam_.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 moveDirection = (camForward * input_.y) + (camRight * input_.x);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        bool isCrouching = _crouch_action.action.IsPressed();
        Vector3 target_scale = isCrouching ? player_crouching_scale : player_standing_scale;
        float cameraPositionY = isCrouching ? camera_crouch_positionY : standing_position_cameraY;
        // float crouch_ = isCrouching ? crouch_speed : _speed;
        // movement //
        // aka Vector3.one;
        transform.localScale = Vector3.Lerp(transform.localScale, target_scale, transition_speed * Time.deltaTime);
        Vector3 cam_pos = cam_.localPosition;
        cam_pos.y = Mathf.Lerp(cam_pos.y, standing_position_cameraY, Time.deltaTime * transition_speed);
        cam_.localPosition = cam_pos;

        if (_controller.collisionFlags == CollisionFlags.Above )
        {
            player_velocity.y = -3.5f;
        }
        bool isMoving = input_.sqrMagnitude > 0.01f;
        bool wantsToSprint = _sprint_action.action.IsPressed() && isMoving && is_grounded && !isCrouching;
        float currentSpeed = _speed;
        if (isCrouching)
        {
            currentSpeed = crouch_speed;
        } else if (wantsToSprint && stamina_ != null && stamina_.canSprint)
        {
            currentSpeed = acceleration_speed;
            stamina_.Sprinting();
        }

            // sprinting //
            // check if the player isn't grounded if so player can't sprint //
        bool isSprinting = _sprint_action.action.IsPressed() && _controller.isGrounded;
        float currentPlayerSpeed = isSprinting ? acceleration_speed : _speed;
        // sprint code done.


        // jump using WasPressedThisFrame();

        if (is_grounded && _jump_action.action.WasPressedThisFrame())
        {
            player_velocity.y = Mathf.Sqrt(jump_height * -2f * GRAVITY);
        }

        // applying gravity 
        player_velocity.y += GRAVITY * Time.deltaTime;
        // moving the player using _controller.Move(); or .SimpleMove();
        Vector3 horizontalMove = moveDirection * currentSpeed * Time.deltaTime;
        Vector3 vertical = Vector3.up * player_velocity.y * Time.deltaTime;
        _controller.Move(horizontalMove + vertical);

       
    } 
}
