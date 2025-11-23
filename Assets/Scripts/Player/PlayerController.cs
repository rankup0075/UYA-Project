using UnityEngine;

/*
    FPS 플레이어 이동/시야 컨트롤러
    - WASD 이동
    - Shift 달리기
    - Space 점프
    - 마우스 시야 회전
    - Raycast 기반 바닥 판정으로 점프 안정화
*/
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 6f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Camera playerCamera;

    private Vector3 velocity;
    private float xRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Look();
        Move();
    }

    // [추가] Raycast 기반 바닥 체크
    private bool IsGrounded()
    {
        // 캐릭터 발 아래에서 아래 방향으로 레이 발사
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float distance = 0.3f; // 바닥과의 거리 체크

        return Physics.Raycast(origin, Vector3.down, distance);
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -55f, 55f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void Move()
    {
        bool running = Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * speed * Time.deltaTime);

        // [수정] CharacterController.isGrounded → Raycast 기반 체크로 변경
        if (IsGrounded())
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
                velocity.y = jumpForce;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
