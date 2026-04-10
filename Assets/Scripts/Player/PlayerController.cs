using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables - Inspector Settings
    [Header("Movement & Physicality")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;
    public float dashForce = 12f;      // Force applied during the dash
    public float dashDuration = 0.2f;   // How long the dash lasts
    public float dashCooldown = 1f;    // Time to wait between dashes

    [Header("Detection & Environment")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float interactionRange = 2f; // Distance for interacting with NPCs/Shops
    public LayerMask interactableLayer;

    [Header("Base RPG Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int denarii = 0;
    public float invincibilityDuration = 0.5f; // Time of protection after being hit

    [Header("Combat - Melee")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 20;     // Base damage for melee attacks
    public float attackRate = 2f;      // Attacks per second
    public LayerMask enemyLayers;      // Layer assigned to enemies

    [Header("Combat - Ranged")]
    public bool isRangedClass = false; // Toggle for Archer-type classes
    public GameObject arrowPrefab;    // Projectile to spawn
    public float arrowSpeed = 20f;
    #endregion

    #region Variables - Private State
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float invincibilityTimer;
    private float nextAttackTime = 0f;
    private bool canDash = true;
    private bool isDashing;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        // Cache necessary components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitializeStats();
    }

    void Update()
    {

        UpdateAnimations();
        // Block actions if currently dashing
        if (isDashing) return;

        HandleInput();
        HandleJump();
        HandleInteraction();
        

        // Check if attack is ready based on cooldown
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1")) // Default Left Mouse or Ctrl
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // Trigger dash if Shift is pressed and cooldown is over
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Handle invincibility countdown
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        HandleInvincibilityVisuals();
    }

    void FixedUpdate()
    {
        // Block physics movement during dash
        if (isDashing) return;

        HandleMovement();
        CheckGround();
    }
    #endregion

    #region Public API (For Other Systems)
    public void TakeDamage(int damage)
    {
        // Ignore damage if player is in invincibility frames
        if (invincibilityTimer > 0) return;

        currentHealth -= damage;
        invincibilityTimer = invincibilityDuration;

        Debug.Log($"Damage taken! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddDenarii(int amount)
    {
        denarii += amount;
        Debug.Log($"Collected {amount} denarii. Total: {denarii}");
    }

    public bool TrySpendDenarii(int amount)
    {
        if (denarii >= amount)
        {
            denarii -= amount;
            return true; // Transaction successful
        }

        Debug.Log("Not enough denarii!");
        return false; // Transaction failed
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

        // Flip the sprite based on movement direction
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Search for all interactable objects within range on the specific layer
            Collider2D[] foundObjects = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

            foreach (Collider2D obj in foundObjects)
            {
                // Try to find a component that implements IInteractable
                IInteractable interactable = obj.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    break; // Stop after the first successful interaction
                }
            }
        }
    }

    private void HandleInvincibilityVisuals()
    {
        if (invincibilityTimer > 0)
        {
            // Create a flashing effect using a Sine wave
            float alpha = Mathf.Sin(Time.time * 20f) > 0 ? 1f : 0.2f;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
        }
        else
        {
            // Reset to full visibility when timer ends
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
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

    private void Attack()
    {
        anim.SetTrigger("Attack");

        if (isRangedClass)
        {
            Shoot();
        }
        else
        {
            MeleeAttack();
        }
    }

    private void Shoot()
    {
        // Spawn the arrow at the designated attack point
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);

        // Apply velocity to the projectile
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        float direction = isFacingRight ? 1f : -1f;
        arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, 0f);

        // Mirror the arrow sprite if shooting left
        if (!isFacingRight)
        {
            arrow.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void MeleeAttack()
    {
        // Detect enemies in a circle at the attack point
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit: " + enemy.name);
            // This is where communication with Enemy scripts will happen
            // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Temporarily disable gravity to prevent falling during dash
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Apply sudden horizontal force
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        // Grant invincibility during the dash duration
        invincibilityTimer = dashDuration;

        yield return new WaitForSeconds(dashDuration);

        // Restore normal physics state
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Wait for cooldown before allowing next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Die()
    {
        Debug.Log("Ave Caesar, morituri te salutant! (The player died)");
        this.enabled = false; // Disable the script
        rb.linearVelocity = Vector2.zero; // Immediately stop movement
    }

    private void UpdateAnimations()
    {
        // Set animator parameters to match current state
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isDashing", isDashing);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip the transform by multiplying local scale by -1
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        // Visualize the ground check area
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Visualize the interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Visualize the melee attack range
        if (attackPoint == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
}