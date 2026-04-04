using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables - Inspector Settings
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;

    [Header("Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("RPG Systems")]
    public int maxHealth = 100;
    public int currentHealth;
    public int denarii = 0;
    #endregion

    #region Variables - Private State
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeStats();
    }

    void Update()
    {
        HandleInput();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }
    #endregion

    #region Public API (For Other Systems)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Damage taken! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool SpendGold(int amount)
    {
        if (denarii >= amount)
        {
            denarii -= amount;
            return true;
        }
        return false;
    }
    #endregion

    #region Private Logic
    private void InitializeStats()
    {
        currentHealth = maxHealth;
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Die()
    {
        Debug.Log("Ave Caesar, morituri te salutant! (The player died)");
        this.enabled = false;
        // Tutaj mo¿na dodaæ np. rb.linearVelocity = Vector2.zero;
    }
    #endregion

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    #endregion


}
