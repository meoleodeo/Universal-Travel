using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthSystem))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private HealthSystem healthSystem;

    public static bool CheatMode = false;

    [Header("Input Settings")]
    [SerializeField] private string playerNum = "1"; // Nhập "1" hoặc "2" trong Inspector
    private string horizontalAxis;
    private string jumpButton;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private float dirX = 0f;
    private float multipleSpeed = 1f;
    private bool isDead = false;
    private bool isKnocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();

        // Thiết lập tên Axis dựa trên playerNum
        // Ví dụ: Horizontal1, Jump1 hoặc Horizontal2, Jump2
        horizontalAxis = "Horizontal" + playerNum;
        jumpButton = "Jump" + playerNum;
    }

    void Update() // Sử dụng Update cho Input để nhạy hơn
    {
        if (isDead || isKnocked) return;

        dirX = Input.GetAxisRaw(horizontalAxis);

        if (Input.GetButtonDown(jumpButton) && IsGrounded())
        {
            Jump();
        }

        // Cheat mode cho phép nhảy liên tục
        if (CheatMode && Input.GetButtonDown(jumpButton))
        {
            Jump();
        }

        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        if (isDead || isKnocked) return;

        // Di chuyển nhân vật
        float finalSpeed = moveSpeed * multipleSpeed;
        rb.linearVelocity = new Vector2(dirX * finalSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.jump);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void UpdateAnimationState()
    {
        if (isDead) return;

        if (!IsGrounded())
        {
            anim.Play("PlayerJump" + playerNum);
        }
        else
        {
            if (dirX > 0f)
            {
                sprite.flipX = false;
                anim.Play("PlayerRun" + playerNum);
            }
            else if (dirX < 0f)
            {
                sprite.flipX = true;
                anim.Play("PlayerRun" + playerNum);
            }
            else
            {
                anim.Play("PlayerIdle" + playerNum);
            }
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public void OnPlayerDied()
    {
        isDead = true;
        rb.linearVelocity = new Vector2(0, 10f);
        // anim.Play("PlayerDeath" + playerNum);
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (isDead) return;
        isKnocked = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine(ResetKnockback());
    }

    private IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        isKnocked = false;
    }

    public void SetSpeedMultiplier(float multiplier) => multipleSpeed = multiplier;
    public string GetPlayerNum()
    {
        return playerNum;
    }
}