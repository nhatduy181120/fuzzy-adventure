using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;

    [Header("Roll Parameters")]
    [SerializeField] private float rollSpeed = 15f;    // Tốc độ lướt
    [SerializeField] private float rollDuration = 0.4f; // Thời gian lướt (giây)
    [SerializeField] private float rollCooldown = 1f;   // Thời gian hồi chiêu roll
    private float rollCounter;
    private float rollCoolCounter;
    private bool isRolling;

    [Header("Cues & Effects")]
    [SerializeField] private GameObject dustPrefab;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private bool wasGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Giảm thời gian hồi chiêu roll theo thời gian thực
        if (rollCoolCounter > 0)
        {
            rollCoolCounter -= Time.deltaTime;
        }

        // CHẾ ĐỘ ĐANG ROLL: Khóa các hành động khác
        if (isRolling)
        {
            rollCounter -= Time.deltaTime;
            if (rollCounter <= 0)
            {
                isRolling = false;
                body.linearVelocity = new Vector2(0, body.linearVelocity.y); // Dừng gia tốc roll
            }
            return; // Thoát Update sớm, không cho di chuyển hay nhảy khi đang roll
        }

        // INPUT DI CHUYỂN BÌNH THƯỜNG
        horizontalInput = Input.GetAxis("Horizontal");

        // Xoay mặt scale
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(6, 6, 6);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-6, 6, 6);
        }

        bool grounded = isGround();

        // Hiệu ứng khói khi tiếp đất
        if (grounded && !wasGrounded)
        {
            CreateDust();
        }
        wasGrounded = grounded;

        // Gán tham số vào Animator
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);

        // KÍCH HOẠT ROLL (Bấm Left Shift, phải đang chạm đất và hết cooldown)
        if (Input.GetKeyDown(KeyCode.LeftShift) && rollCoolCounter <= 0 && grounded)
        {
            StartRoll();
            return; // Bỏ qua phần logic nhảy/di chuyển phía dưới ở khung hình này
        }

        // Logic Nhảy và Nhảy tường
        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * moveSpeed, body.linearVelocity.y);

            if (onWall() && !grounded)
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = 7;

            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    private void StartRoll()
    {
        isRolling = true;
        rollCounter = rollDuration;
        rollCoolCounter = rollCooldown;

        anim.SetTrigger("Roll"); // Kích hoạt Animation Roll trong Unity
        CreateDust(); // Tạo tí khói lúc bắt đầu lướt cho ngầu

        // Xác định hướng lướt dựa trên hướng mặt nhân vật (scale x)
        float rollDirection = Mathf.Sign(transform.localScale.x);
        body.linearVelocity = new Vector2(rollDirection * rollSpeed, body.linearVelocity.y);
    }

    private void Jump()
    {
        if (isGround())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            anim.SetTrigger("Jump");
            CreateDust();
        }
        else if (onWall() && !isGround())
        {
            if (horizontalInput == 0)
            {
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * 6, transform.localScale.y, transform.localScale.z);
            }
            else
                body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            wallJumpCooldown = 0;
        }
    }

    private void CreateDust()
    {
        if (dustPrefab != null)
        {
            // Tạo ra dust tại vị trí chân nhân vật (boxCollider.bounds.min.y)
            Vector3 spawnPos = new Vector3(transform.position.x, boxCollider.bounds.min.y, transform.position.z);
            GameObject dust = Instantiate(dustPrefab, spawnPos, Quaternion.identity);
            Destroy(dust, 0.5f); // Tự hủy sau 0.5 giây (hoặc tùy độ dài animation)
        }
    }

    private bool isGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        float faceDirection = Mathf.Sign(transform.localScale.x);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(faceDirection, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        // Không cho phép attack khi đang roll
        return horizontalInput == 0 && isGround() && !onWall() && !isRolling;
    }
}