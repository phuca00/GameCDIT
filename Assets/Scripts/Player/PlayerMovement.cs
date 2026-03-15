using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;

    [Header("Move & Jump")]
    public float speed = 4f;
    public float jumpingPower = 8f;

    [Header("Rigidbody & Checks")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public LayerMask wallLayer;

    [Header("Ground Check Settings")]
    public float groundCheckRadius = 0.18f;    // <-- điều chỉnh (0.15-0.25)
    public float coyoteTime = 0.12f;           // thời gian vẫn coi là grounded sau khi rời đất
    public float jumpBufferTime = 0.12f;       // thời gian buffer khi bấm jump trước khi chạm đất

    private float coyoteTimeCounter = 0f;
    private float jumpBufferCounter = 0f;

    [Header("Jump Counters")]
    private bool canDoubleJump = false;
    private int wallJumpCount = 0;
    public int maxWallJumps = 2;   // 2 lần wall jump

    [Header("Wall Sliding")]
    private bool isWallSliding = false;
    public float wallSlidingSpeed = 2f;

    [Header("Wall Jump")]
    private bool isWallJumping = false;
    public float wallJumpDirection;
    public float wallJumpDuration = 0.2f;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Animation")]
    public Animator animator;
    private static readonly int stateHash = Animator.StringToHash("state");

    private enum State { idle, running, jumping, falling, doubleJump, wallSlide }

    // cached grounded flag (determined in FixedUpdate)
    private bool isGroundedCached = false;

    private void Update()
    {
        if (Pause.inputLocked)
        {
            horizontal = 0;
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        // Jump input buffering
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Variable jump height on release
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.55f);

        // Try perform jump if buffered and conditions met
        TryConsumeJumpBuffer();

        WallSlide();
        Flip();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // Ground check should run in FixedUpdate for physics consistency
        CheckGround();

        if (!isWallJumping)
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        // coyote time: count down
        if (isGroundedCached)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;
    }

    // ============================================================
    // GROUND CHECK (cached)
    // ============================================================
    private void CheckGround()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (grounded && !isGroundedCached)
        {
            // just landed
            canDoubleJump = true;
            wallJumpCount = 0;
        }

        isGroundedCached = grounded;
    }

    private bool IsGrounded()
    {
        // Return true if actually grounded OR still within coyote time
        return isGroundedCached || coyoteTimeCounter > 0f;
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    // ============================================================
    // JUMP HANDLING (with buffer + coyote)
    // ============================================================
    private void TryConsumeJumpBuffer()
    {
        // If there was a buffered jump and enough conditions, do it
        if (jumpBufferCounter > 0f)
        {
            // 1) Grounded jump
            if (IsGrounded())
            {
                DoJump();
                jumpBufferCounter = 0f;
                return;
            }

            // 2) Wall jump (allow if touching wall and not grounded)
            if (IsWalled() && wallJumpCount < maxWallJumps)
            {
                wallJumpCount++;
                StartWallJump();
                jumpBufferCounter = 0f;
                return;
            }

            // 3) Double jump (if available)
            if (canDoubleJump && !IsGrounded())
            {
                canDoubleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower * 0.9f);
                AudioManager.instance.PlayJump();
                jumpBufferCounter = 0f;
                return;
            }
        }
    }

    private void DoJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        AudioManager.instance.PlayJump();
        // when jump, consume coyote
        coyoteTimeCounter = 0f;
    }

    // ============================================================
    // WALL SLIDE
    // ============================================================
    private void WallSlide()
    {
        // improved condition: want to slide only when pushing toward wall or holding horizontal input
        bool touchingWall = IsWalled();
        bool pushingTowardsWall = (horizontal > 0 && transform.localScale.x > 0 && touchingWall)
                                 || (horizontal < 0 && transform.localScale.x < 0 && touchingWall);

        if (touchingWall && !IsGrounded() && (Mathf.Abs(horizontal) > 0f || pushingTowardsWall))
        {
            isWallSliding = true;

            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    // ============================================================
    // WALL JUMP
    // ============================================================
    private void StartWallJump()
    {
        isWallSliding = false;
        isWallJumping = true;

        // wallJumpDirection: push away from wall (use localScale.x as facing)
        wallJumpDirection = -transform.localScale.x;

        rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);

        // flip player to face jump direction
        transform.localScale = new Vector3(wallJumpDirection, 1, 1);

        Invoke(nameof(StopWallJump), wallJumpDuration);
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // ============================================================
    // FLIP
    // ============================================================
    private void Flip()
    {
        if (isWallJumping) return;

        if (horizontal > 0) transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        if (horizontal < 0) transform.localScale = new Vector3(-1.4f, 1.4f, 1.4f);
    }

    // ============================================================
    // ANIMATION
    // ============================================================
    private void UpdateAnimation()
    {
        State state;

        // ƯU TIÊN 1: Nếu grounded → Idle / Run
        if (IsGrounded())
        {
            if (Mathf.Abs(horizontal) > 0.1f)
                state = State.running;
            else
                state = State.idle;

            animator.SetInteger(stateHash, (int)state);
            return; // STOP ở đây luôn, không xét các anim khác nữa
        }

        // ƯU TIÊN 2: Wall slide
        if (isWallSliding)
        {
            state = State.wallSlide;
        }
        // ƯU TIÊN 3: Jump upward
        else if (rb.velocity.y > 0.1f)
        {
            state = canDoubleJump ? State.jumping : State.doubleJump;
        }
        // ƯU TIÊN 4: Fall
        else if (rb.velocity.y < -0.1f)
        {
            state = State.falling;
        }
        else
        {
            state = State.idle;
        }

        animator.SetInteger(stateHash, (int)state);
    }


    // Optional: debug gizmos for ground check
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
        }
    }
}
