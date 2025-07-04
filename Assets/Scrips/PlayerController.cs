using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- CÁC THÔNG SỐ CƠ BẢN ---
    [SerializeField] private float maxSpeed = 50f;         // Tốc độ tối đa không thể vượt quá
    [SerializeField] private float jumpForce = 30f;        // Lực nhảy - càng lớn nhảy càng cao
    [SerializeField] private float torqueAmount = 8f;    // Độ nhạy khi xoay - càng lớn xoay càng nhanh
    [SerializeField] private LayerMask groundLayer;        // Layer của mặt đất/tuyết
    [SerializeField] private Transform groundCheck;        // Điểm kiểm tra va chạm với đất

    // --- THÔNG SỐ VẬT LÝ ---
    [SerializeField] private float decelerationRate = 2.2f;       // Độ ma sát - càng lớn dừng càng nhanh
    [SerializeField] private float minSpeedThreshold = 2f;      // Tốc độ tối thiểu để duy trì chuyển động
    [SerializeField] private float gravityMultiplier = 2.2f;    // Hệ số trọng lực - càng lớn càng nặng
    [SerializeField] private float moveForce = 10f;           

    // --- HỆ THỐNG CÂN BẰNG ---
    [SerializeField] private float balanceForce = 3f;         // Lực giữ thăng bằng - càng lớn càng khó nghiêng
    [SerializeField] private float maxBalanceAngle = 45f;       // Góc nghiêng tối đa cho phép
    [SerializeField] private float balanceSpeed = 25f;          // Tốc độ cân bằng lại - càng lớn càng nhanh
    [SerializeField] private float stabilityForce = 3f;         // Lực ổn định - giúp không bị lật

    // --- ĐIỀU KHIỂN TRÊN KHÔNG ---
    [SerializeField] private float airRotationSpeed = 720f;     // Tốc độ xoay cơ bản khi nhấn W/S trên không
    [SerializeField] private float maxAirRotationSpeed = 500f;  // Tốc độ xoay tối đa trên không

    // --- BIẾN PRIVATE ---
    private bool isGrounded;           // Kiểm tra có đang chạm đất không
    private Rigidbody2D rb;           // Component điều khiển vật lý
    private Animator animator;         // Component điều khiển animation
    //
    [SerializeField] private GameObject helmetShieldPrefab;
    private GameObject helmetShieldInstance;
    private Animator shieldAnim;
    private bool isShieldActive = false;
    private bool isPaused = false;

    //Cho kiểm tra xoay
    private bool wasGroundedLastFrame = true;
    private float lastZRotation = 0f;
    private float accumulatedRotation = 0f;
    private int spinCount = 0;

    private float airborneTime = 0f;
    private float airborneThreshold = 0.5f; // Thời gian tối thiểu ở trên không để bắt đầu tính spin

    private bool isGamePaused = false;
    private bool isGameOverOrWin = false;
    private bool isGameOver = false;

    private void Awake()
    {
        // Khởi tạo các component cần thiết
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Thiết lập các thông số vật lý cơ bản
        rb.gravityScale = 2f * gravityMultiplier;  // Điều chỉnh trọng lực
        rb.linearDamping = 0.001f;                 // Giảm ma sát khi di chuyển
        rb.angularDamping = 0.5f;                  // Giảm ma sát khi xoay
        rb.mass = 2f;
        // Khối lượng của nhân vật

        rb.constraints = RigidbodyConstraints2D.None;  // Không giới hạn chuyển động
        rb.freezeRotation = false;

        // Cho phép xoay
    }

    void Update()
    {
        // Cập nhật isGrounded trước tiên
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.6f, groundLayer);
        //Debug.Log($"Frame: isGrounded={isGrounded}, wasGroundedLastFrame={wasGroundedLastFrame}");


        // Kiểm tra nếu game bị pause thì không xử lý input
        if (Time.timeScale == 0f)
        {
            isPaused = true;
            return;
        }

        if (isPaused && Time.timeScale > 0f)
        {
            // Game vừa được resume, reset isPaused
            isPaused = false;
        }

        HandleMovement();
        HandleJump();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Không xử lý physics khi game bị pause
        if (Time.timeScale == 0f)
            return;

        // Luôn kiểm tra và giới hạn tốc độ trước
        if (!isSpeedBoosting && !isDeceleratingAfterBoost)
        {
            if (rb.linearVelocity.magnitude > maxSpeed)
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
        float rotationInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(rb.angularVelocity) < maxAirRotationSpeed)
        {
            rb.AddTorque(-rotationInput * airRotationSpeed * Time.fixedDeltaTime);
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
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
            if (hit.collider != null)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                float slopeDir = Mathf.Sign(hit.normal.x);

                // Áp dụng lực trượt dựa trên độ dốc
                if (rb.linearVelocity.magnitude < maxSpeed)
                {
                    Vector2 moveDir = new Vector2(slopeDir, 0);
                    float speedMultiplier = 1f + (slopeAngle / 45f); // Tăng tốc độ dựa trên độ dốc
                    rb.AddForce(moveDir * moveForce * speedMultiplier);
                }

                // Áp dụng xoay dựa trên độ dốc
                float torqueMultiplier = 1f + (slopeAngle / 45f);
                rb.AddTorque(-slopeDir * torqueAmount * torqueMultiplier);

                // Áp dụng giảm tốc khi độ dốc nhỏ
                if (slopeAngle < 5f)
                {
                    rb.AddForce(-rb.linearVelocity.normalized * decelerationRate);
                }
            }
        }
        else
        {
            HandleAirRotation();
        }

        // Cập nhật hướng nhìn dựa trên hướng di chuyển
        if (rb.linearVelocity.x > 0)
            transform.localScale = new Vector3(0.4f, 0.45f, 1);
        else if (rb.linearVelocity.x < 0)
            transform.localScale = new Vector3(-0.4f, 0.45f, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void UpdateAnimation()
    {
        bool isSliding = Mathf.Abs(rb.linearVelocity.x) > minSpeedThreshold;
        bool isJumping = !isGrounded;

        animator.SetBool("isSliding", isSliding);
        animator.SetBool("isJumping", isJumping);
    }

    public float GetCurrentSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    private Coroutine slowCoroutine;
    private float originalDrag = 0f;

    public void ReduceSpeedTemporarily(float factor, float duration)
    {
        if (isShieldActive) return; // Miễn nhiễm nếu có khiên

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowDownCoroutine(factor, duration));
    }

    private IEnumerator SlowDownCoroutine(float factor, float duration)
    {
        rb.linearVelocity *= factor;
        originalDrag = rb.linearDamping;
        rb.linearDamping += 5f;
        yield return new WaitForSeconds(duration);
        rb.linearDamping = originalDrag;
    }
    public void ReduceSpeed(float factor)
    {
        if (rb != null)
        {
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    /// tăng tốc
    // Biến hỗ trợ tăng tốc
    private Coroutine boostCoroutine;
    private float originalDamping = 0f;
    private Vector2 originalVelocityBeforeBoost;
    private bool isSpeedBoosting = false;
    private bool isDeceleratingAfterBoost = false;
    [SerializeField] private float decelerateDuration = 1.5f; // Thời gian giảm dần về tốc độ ban đầu

    // Gọi từ item tăng tốc
    public void BoostSpeedTemporarily(float boostMultiplier, float duration)
    {
        if (boostCoroutine != null)
            StopCoroutine(boostCoroutine);

        boostCoroutine = StartCoroutine(SpeedBoostCoroutine(boostMultiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostMultiplier, float duration)
    {
        if (rb.linearVelocity.magnitude < 0.1f)
            yield break; // Không boost nếu nhân vật đang đứng yên

        isSpeedBoosting = true;

        // Lưu lại trạng thái ban đầu
        originalVelocityBeforeBoost = rb.linearVelocity;
        originalDamping = rb.linearDamping;

        float boostX = rb.linearVelocity.x * boostMultiplier;
        float boostY = rb.linearVelocity.y;
        Vector2 targetVelocity = new Vector2(boostX, boostY);

        // Clamp không vượt quá maxSpeed
        if (targetVelocity.magnitude > maxSpeed)
            targetVelocity = targetVelocity.normalized * maxSpeed;

        float elapsed = 0f;

        // Giảm damping để tăng tốc mượt mà
        rb.linearDamping = 0.1f;

        // Tăng tốc dần (Ramp-up) - Sử dụng deltaTime thay vì fixedDeltaTime
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime; // Sử dụng scaled time
            float t = Mathf.Clamp01(elapsed / duration);
            rb.linearVelocity = Vector2.Lerp(originalVelocityBeforeBoost, targetVelocity, t);

            yield return null; // Chờ frame tiếp theo thay vì FixedUpdate
        }

        // Trả lại damping gốc và bắt đầu giảm tốc
        rb.linearDamping = originalDamping;
        yield return StartCoroutine(GradualDecelerateToOriginal());

        isSpeedBoosting = false;
    }

    private IEnumerator GradualDecelerateToOriginal()
    {
        isDeceleratingAfterBoost = true;
        float elapsed = 0f;
        Vector2 startVelocity = rb.linearVelocity;
        Vector2 targetVelocity = originalVelocityBeforeBoost;

        while (elapsed < decelerateDuration)
        {
            elapsed += Time.deltaTime; // Sử dụng scaled time
            float t = elapsed / decelerateDuration;

            // Easing mượt hơn (ease-out)
            float easeOutT = 1f - Mathf.Pow(1f - t, 2f);
            rb.linearVelocity = Vector2.Lerp(startVelocity, targetVelocity, easeOutT);

            if ((rb.linearVelocity - targetVelocity).magnitude < 0.05f)
                break;

            yield return null; // Chờ frame tiếp theo thay vì FixedUpdate
        }

        rb.linearVelocity = targetVelocity;
        isDeceleratingAfterBoost = false;
    }

    //
    public void ActivateShield(float duration)
    {

        if (isShieldActive)
        {
            Debug.Log("Shield already active.");
            return;
        }

        Debug.Log("Shield activated for " + duration + " seconds.");

        helmetShieldInstance = Instantiate(helmetShieldPrefab, transform);
        helmetShieldInstance.transform.localPosition = Vector3.zero;

        shieldAnim = helmetShieldInstance.GetComponent<Animator>();
        if (shieldAnim == null)
        {
            Debug.LogWarning("Animator not found on helmet shield prefab.");
            Destroy(helmetShieldInstance);
            return;
        }

        isShieldActive = true;
        SetItemCollidersActive(false); // Tắt va chạm Item
        StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        shieldAnim.Play("ShieldAppear");

        yield return new WaitUntil(() =>
            shieldAnim != null && shieldAnim.GetCurrentAnimatorStateInfo(0).IsName("ShieldIdle"));

        float idleTime = duration - 2f;
        if (idleTime > 0)
            yield return new WaitForSeconds(idleTime);

        if (helmetShieldInstance != null && shieldAnim != null)
        {
            shieldAnim.SetTrigger("FadeOut");
            yield return new WaitForSeconds(2f);
        }

        if (helmetShieldInstance != null)
            Destroy(helmetShieldInstance);

        helmetShieldInstance = null;
        shieldAnim = null;
        isShieldActive = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.getShieldDisappear);
        SetItemCollidersActive(true);

        StartCoroutine(DelayedReactivation());
    }

    private bool isShieldCooldown = false;

    public bool IsInvulnerable()
    {
        return isShieldActive || isShieldCooldown;
    }

    public void ForceDeactivateShield()
    {
        if (!isShieldActive) return;

        StopCoroutine("ShieldRoutine");

        if (helmetShieldInstance != null)
            Destroy(helmetShieldInstance);

        helmetShieldInstance = null;
        shieldAnim = null;
        isShieldActive = false;

        StartCoroutine(DelayedReactivation());

        StartCoroutine(ShieldCooldownBuffer());
        Debug.Log("Khiên đã bị hủy sớm do va chạm.");
        
    }
    private IEnumerator ShieldCooldownBuffer()
    {
        isShieldCooldown = true;
        yield return new WaitForSeconds(1f); // Khoảng đệm để tránh va chạm kép
        isShieldCooldown = false;
    }
   

    private IEnumerator DelayedReactivation()
    {
        yield return new WaitForSeconds(0.1f); // để player rời khỏi vùng va chạm
        SetItemCollidersActive(true);
    }

    // Hàm public để GameManager có thể gọi khi pause/resume
    public void OnGamePaused()
    {
        isPaused = true;
    }

    public void OnGameResumed()
    {
        isPaused = false;
    }

    public void OnGameOverOrWin()
    {
        isGameOverOrWin = true;
    }

    public void OnRestartOrNextLevel()
    {
        isGameOverOrWin = false;
    }

    public void OnGameOver()
    {
        isGameOver = true;
    }

    void LateUpdate()
    {
        if (isGamePaused || isGameOverOrWin || isGameOver)
            return;

        // Vừa rời khỏi mặt đất (bắt đầu nhảy)
        if (wasGroundedLastFrame && !isGrounded)
        {
            accumulatedRotation = 0f;
            spinCount = 0;
            lastZRotation = transform.eulerAngles.z;
            airborneTime = 0f;
        }

        // Đang ở trên không
        if (!isGrounded)
        {
            airborneTime += Time.deltaTime;
            if (airborneTime > airborneThreshold)
            {
                float currentZ = transform.eulerAngles.z;
                float delta = Mathf.DeltaAngle(lastZRotation, currentZ);
                accumulatedRotation += Mathf.Abs(delta);
                lastZRotation = currentZ;

                while (accumulatedRotation >= 360f)
                {
                    spinCount++;
                    accumulatedRotation -= 360f;
                }
                //Debug.Log($"[SPIN] accumulatedRotation: {accumulatedRotation}° | spinCount: {spinCount}");
            }
        }

        // Vừa tiếp đất
        if (!wasGroundedLastFrame && isGrounded)
        {
            // Tính số vòng hoàn chỉnh kể cả vòng dư
            float totalRotation = spinCount * 360f + accumulatedRotation;
            int totalSpin = Mathf.FloorToInt((totalRotation + 90f) / 360f); // +30 để dễ ăn bonus khi suýt đủ 1 vòng

            if (totalSpin > 0)
                //Debug.Log($"[SPIN] Sắp gọi AddSpinScore({totalSpin})");
            ScoreManager.Instance.AddSpinScore(totalSpin);
            //Debug.Log($"[SPIN] Đã gọi AddSpinScore({totalSpin})");

            //Debug.Log($"[SPIN] OnLand - accumulatedRotation: {accumulatedRotation}° | spinCount: {spinCount}");

            spinCount = 0;
            accumulatedRotation = 0f;
            lastZRotation = transform.eulerAngles.z;
            airborneTime = 0f;
        }
        wasGroundedLastFrame = isGrounded;
    }

    public void ResetSpinState()
    {
        wasGroundedLastFrame = true;
        spinCount = 0;
        accumulatedRotation = 0f;
        lastZRotation = transform.eulerAngles.z;
    }

    private void SetItemCollidersActive(bool isActive)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (var item in items)
        {
            if (item == null) continue;

            var col = item.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = isActive;
        }
    }
}
