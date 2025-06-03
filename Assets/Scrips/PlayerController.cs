using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- CÁC THÔNG SỐ CƠ BẢN ---
    [SerializeField] private float baseSpeed = 6f;        // Tốc độ cơ bản khi trượt
    [SerializeField] private float maxSpeed = 20f;         // Tốc độ tối đa không thể vượt quá
    [SerializeField] private float jumpForce = 10f;        // Lực nhảy - càng lớn nhảy càng cao
    [SerializeField] private float torqueAmount = 1.2f;    // Độ nhạy khi xoay - càng lớn xoay càng nhanh
    [SerializeField] private LayerMask groundLayer;        // Layer của mặt đất/tuyết
    [SerializeField] private Transform groundCheck;        // Điểm kiểm tra va chạm với đất
    
    // --- THÔNG SỐ VẬT LÝ ---
    [SerializeField] private float accelerationRate = 30f;      // Tốc độ tăng tốc - càng lớn tăng tốc càng nhanh
    [SerializeField] private float decelerationRate = 2f;       // Độ ma sát - càng lớn dừng càng nhanh
    [SerializeField] private float minSpeedThreshold = 2f;      // Tốc độ tối thiểu để duy trì chuyển động
    [SerializeField] private float slopeSpeedMultiplier = 6f;   // Hệ số tăng tốc khi xuống dốc
    [SerializeField] private float gravityMultiplier = 2.2f;    // Hệ số trọng lực - càng lớn càng nặng
    [SerializeField] private float minSlopeAngle = 5f;          // Góc dốc tối thiểu để bắt đầu trượt tự động
    [SerializeField] private float moveForce = 60f;             // Lực di chuyển khi nhấn A/D - càng lớn di chuyển càng nhanh
    
    // --- HỆ THỐNG CÂN BẰNG ---
    [SerializeField] private float balanceForce = 3.5f;           // Lực giữ thăng bằng - càng lớn càng khó nghiêng
    [SerializeField] private float maxBalanceAngle = 35f;       // Góc nghiêng tối đa cho phép
    [SerializeField] private float balanceSpeed = 10f;          // Tốc độ cân bằng lại - càng lớn càng nhanh
    [SerializeField] private float stabilityForce = 6f;         // Lực ổn định - giúp không bị lật

    // --- ĐIỀU KHIỂN TRÊN KHÔNG ---
    [SerializeField] private float airRotationSpeed = 180f;     // Tốc độ xoay cơ bản khi nhấn W/S trên không
    [SerializeField] private float maxAirRotationSpeed = 250f;  // Tốc độ xoay tối đa trên không
    
    // --- BIẾN PRIVATE ---
    private bool isGrounded;           // Kiểm tra có đang chạm đất không
    private Rigidbody2D rb;           // Component điều khiển vật lý
    private float currentSpeed;        // Tốc độ hiện tại
    private Animator animator;         // Component điều khiển animation
    private float lastSlopeAngle;     // Góc dốc cuối cùng
    private Vector2 lastVelocity;     // Vận tốc cuối cùng

    private void Awake()
    {
        // Khởi tạo các component cần thiết
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
        
        // Thiết lập các thông số vật lý cơ bản
        rb.gravityScale = 2f * gravityMultiplier;  // Điều chỉnh trọng lực
        rb.linearDamping = 0.001f;                 // Giảm ma sát khi di chuyển
        rb.angularDamping = 0.5f;                  // Giảm ma sát khi xoay
        rb.mass = 2f;                             // Khối lượng của nhân vật
        
        rb.constraints = RigidbodyConstraints2D.None;  // Không giới hạn chuyển động
        rb.freezeRotation = false;                     // Cho phép xoay
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimation();
        lastVelocity = rb.linearVelocity;

        // Debug logs
        Debug.Log($"Tốc độ hiện tại: {rb.linearVelocity.magnitude:F2} | " +
                  $"Input ngang: {Input.GetAxis("Horizontal"):F2} | " +
                  $"Input dọc: {Input.GetAxis("Vertical"):F2} | " +
                  $"Đang chạm đất: {isGrounded}");
    }

    void FixedUpdate()
    {
        // Luôn kiểm tra và giới hạn tốc độ trước
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (isGrounded)
        {
            ApplyBalance();
            ApplyStability();
        }
        else
        {
            HandleAirRotation();
        }
    }

    private void HandleAirRotation()
    {
        float rotationInput = Input.GetAxis("Vertical"); // W = 1, S = -1

        if (Mathf.Abs(rb.angularVelocity) < maxAirRotationSpeed)
        {
            rb.AddTorque(-rotationInput * airRotationSpeed * Time.fixedDeltaTime); // Đảo dấu để W = xoay về trước
        }

        // Giảm xoay dần
        if (Mathf.Abs(rb.angularVelocity) > 0.1f)
            rb.angularVelocity *= 0.98f;
    }


    private void ApplyStability()
    {
        if (!isGrounded) return;

        float currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        if (Mathf.Abs(currentAngle) > 20f)
        {
            float stabilizingTorque = -Mathf.Sign(currentAngle) * stabilityForce;
            rb.AddTorque(stabilizingTorque * Time.fixedDeltaTime);
        }

        if (Mathf.Abs(rb.angularVelocity) > 50f)
        {
            rb.angularVelocity *= 0.92f;
        }
    }

    private void ApplyBalance()
    {
        if (!isGrounded) return;

        float currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
        if (hit.collider != null)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            float targetAngle = -slopeAngle * Mathf.Sign(hit.normal.x);

            targetAngle = Mathf.Clamp(targetAngle, -maxBalanceAngle, maxBalanceAngle);

            float angleError = Mathf.DeltaAngle(currentAngle, targetAngle);
            float balanceTorque = angleError * balanceForce;

            if (Mathf.Abs(angleError) < 15f)
            {
                balanceTorque *= 1.5f;
            }

            rb.AddTorque(balanceTorque * Time.fixedDeltaTime * balanceSpeed);
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
            if (hit.collider != null)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                float slopeDir = -Mathf.Sign(hit.normal.x);

                // Lực trượt tự nhiên theo độ dốc
                if (slopeAngle > minSlopeAngle)
                {
                    Vector2 slopeVector = new Vector2(-hit.normal.y, hit.normal.x) * slopeDir;
                    float slopeForce = baseSpeed * (slopeAngle / 45f);
                    
                    // Kiểm tra tốc độ trước khi thêm lực
                    if (rb.linearVelocity.magnitude < maxSpeed)
                    {
                        rb.AddForce(slopeVector * slopeForce);

                        // Tăng tốc thêm khi xuống dốc
                        if (Vector2.Dot(rb.linearVelocity, slopeVector) > 0)
                        {
                            rb.AddForce(slopeVector * slopeSpeedMultiplier);
                        }
                    }
                }

                // Xử lý input A/D
                if (Mathf.Abs(moveInput) > 0.1f)
                {
                    // Chỉ áp dụng lực nếu chưa đạt tốc độ tối đa
                    if (rb.linearVelocity.magnitude < maxSpeed)
                    {
                        Vector2 moveDir = new Vector2(moveInput, 0);
                        float speedMultiplier = 1f;

                        if (moveInput * slopeDir > 0)
                        {
                            speedMultiplier = 1.5f;
                        }
                        else
                        {
                            speedMultiplier = 0.7f;
                        }

                        rb.AddForce(moveDir * moveForce * speedMultiplier);
                    }

                    // Vẫn cho phép xoay ngay cả khi đạt tốc độ tối đa
                    float torqueMultiplier = 1f + (slopeAngle / 45f);
                    rb.AddTorque(-moveInput * torqueAmount * torqueMultiplier);
                }
                else
                {
                    rb.AddForce(-rb.linearVelocity.normalized * decelerationRate * 0.5f);
                }
            }
        }
        else
        {
            HandleAirRotation();
        }

        // Cập nhật hướng nhìn
        if (moveInput > 0)
            transform.localScale = new Vector3(0.4f, 0.45f, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-0.4f, 0.45f, 1);
    }


    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Nhảy!");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void UpdateAnimation()
    {
        // Tăng ngưỡng tốc độ để xác định trạng thái trượt
        bool isSliding = Mathf.Abs(rb.linearVelocity.x) > minSpeedThreshold;
        bool isJumping = !isGrounded;

        // Thêm debug để kiểm tra
        Debug.Log($"Animation State | Tốc độ X: {Mathf.Abs(rb.linearVelocity.x):F2} | " +
                  $"Ngưỡng: {minSpeedThreshold} | " +
                  $"isSliding: {isSliding} | " +
                  $"isJumping: {isJumping}");

        animator.SetBool("isSliding", isSliding);
        animator.SetBool("isJumping", isJumping);
    }
}
