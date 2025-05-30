using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float Speed = 8;
    public float JumpHeight = 5;
    public float GroundCheckRadius = 0.2f;
    public int MaxJumpCount = 2;
    private int JumpCount = 1;
    private bool IsGrounded;

    public Animator animator;
    public Transform GroundCheck;
    public LayerMask GroundLayer;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, GroundLayer);
        if (IsGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            JumpCount = 0;
        }
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        PlayerMovement(HorizontalInput);
        PlayerHorizontalMovementAnimation(HorizontalInput);
    }

    private void PlayerMovement(float HorizontalInput)
    {
        rb.linearVelocity = new Vector2(HorizontalInput * Speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && JumpCount < MaxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0f, JumpHeight), ForceMode2D.Impulse);
            JumpCount++;
        }
    }

    private void PlayerHorizontalMovementAnimation(float HorizontalInput)
    {
        animator.SetFloat("Speed", Mathf.Abs(HorizontalInput));
        Vector3 scale = transform.localScale;
        if (HorizontalInput < 0)
        {
            scale.x = -1 * Mathf.Abs(scale.x);
        }
        else if (HorizontalInput > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
    }
}
