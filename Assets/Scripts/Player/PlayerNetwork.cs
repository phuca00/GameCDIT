using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;
    public float jumpingPower = 8f;
    public float wallSlidingSpeed = 2f;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    public float wallJumpDuration = 0.2f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public LayerMask wallLayer;

    // --- BIẾN LOGIC NỘI BỘ ---
    private float horizontal;
    private bool isFacingRight = true; 
    private bool isWallSliding; 
    private bool isWallJumping;
    private float wallJumpDirection;
    private bool canDoubleJump;
    
    private bool isDoingDoubleJump; 
    
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    private int lastSentState = -1;

    private static readonly int stateHash = Animator.StringToHash("state");

    // --- BIẾN MẠNG ---
    private NetworkVariable<int> netState = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<float> netScaleX = new NetworkVariable<float>(1.4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = true; 
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (IsOwner)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f; 
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; 
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic; 
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero; 
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; 
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            if (animator != null) animator.SetInteger(stateHash, netState.Value);
            transform.localScale = new Vector3(netScaleX.Value, 1.4f, 1.4f);
            return; 
        }

        if (Pause.inputLocked) { horizontal = 0; return; }
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded()) 
        {
            coyoteTimeCounter = coyoteTime;
            isDoingDoubleJump = false; // Chạm đất là chắc chắn ngắt nhảy đôi
        }
        else 
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f) 
            {
                jumpBufferCounter = 0f; 
                coyoteTimeCounter = 0f; 
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower); 
                canDoubleJump = true;
                isDoingDoubleJump = false;
            }
            else if (isWallSliding) 
            {
                jumpBufferCounter = 0f; 
                isWallJumping = true;
                wallJumpDirection = isFacingRight ? -1f : 1f;
                rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                Invoke(nameof(StopWallJumping), wallJumpDuration);
                isDoingDoubleJump = false; 
            }
            else if (canDoubleJump) 
            {
                jumpBufferCounter = 0f; 
                canDoubleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower * 0.85f);
                isDoingDoubleJump = true; // Kích hoạt nhảy đôi
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        WallSlideLogic(); 
        Flip();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || isWallJumping) return;
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void StopWallJumping() => isWallJumping = false;

    private void WallSlideLogic()
    {
        if (IsWalled() && !IsGrounded() && Mathf.Abs(horizontal) > 0.1f)
        {
            isWallSliding = true;
            isDoingDoubleJump = false; 
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void UpdateAnimation()
    {
        int s = 0; 
        
        if (isWallSliding) 
        {
            s = 5; // 5: Trượt tường
        }
        else if (!IsGrounded()) 
        {
            if (isDoingDoubleJump) 
            {
                s = 4; // 4: Nhảy đôi (Xoay tròn)
                
                // MẸO GAME FEEL: Bay đến điểm cao nhất, bắt đầu rơi thì ngắt trạng thái xoay!
                if (rb.velocity.y < 0f) isDoingDoubleJump = false;
            }
            else 
            {
                s = rb.velocity.y > 0.1f ? 2 : 3; // 2: Nhảy thường, 3: Rơi
            }
        }
        else 
        {
            s = Mathf.Abs(horizontal) > 0.1f ? 1 : 0; // 1: Chạy, 0: Đứng
        }

        animator.SetInteger(stateHash, s);

        if (s != lastSentState)
        {
            lastSentState = s;
            UpdateAnimServerRpc(s);
        }
    }

    [ServerRpc] 
    void UpdateAnimServerRpc(int s) => netState.Value = s;

    private void Flip()
    {
        if (IsWalled() || isWallJumping) return; 

        if (horizontal > 0.1f && !isFacingRight) ExecuteFlip(true);
        else if (horizontal < -0.1f && isFacingRight) ExecuteFlip(false);
    }

    private void ExecuteFlip(bool right)
    {
        isFacingRight = right; 
        float newScale = right ? 1.4f : -1.4f;

        transform.localScale = new Vector3(newScale, 1.4f, 1.4f);
        UpdateScaleServerRpc(newScale);
    }

    [ServerRpc] 
    void UpdateScaleServerRpc(float s) => netScaleX.Value = s;

    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    private bool IsWalled() => Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
}