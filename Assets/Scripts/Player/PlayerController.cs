using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables - Inspector Settings
    [Header("Class Configuration")]
    public CharacterClassData classData;

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
    public float invincibilityDuration = 0.5f;// Time of protection after being hit
    public float attackRate = 2f;
    public float criticalChance = 0.1f; // 10% base critical hit chance
    private PlayerStats stats; 

    [Header("Combat - Melee")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 20;     // Base damage for melee attacks
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
    private IInteractable currentInteractable;

    private float invincibilityTimer;
    private float nextAttackTime = 0f;
    private float nextSpecialAttackTime = 0f;
    private float lastSpecialCooldownDuration = 1f; // Storage for dynamic cooldown length
    private bool canDash = true;
    private bool isDashing;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = PlayerStats.Instance;

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


        // 1. Basic Attack Cooldown
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // 2. Special Attack Cooldown (Fire2) 
        if (Input.GetButtonDown("Fire2") && stats.hasSpecialAbility && Time.time >= nextSpecialAttackTime)
        {
            if (classData.className == "Archer")
            {
                lastSpecialCooldownDuration = (1f / attackRate) * 1.5f;
                nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration; 
                MultiShot();
            }
            else if (classData.className == "Legionary")
            {
                lastSpecialCooldownDuration = (1f / attackRate) * 2f;
                nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration; 
                VanguardStrike();
            }
            else if (classData.className == "Gladiator")
            {
                lastSpecialCooldownDuration = 15f;
                nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration; 
                SpartanRage();
            }

            //Trigger a short basic attack cooldown to prevent animation overlap
            nextAttackTime = Time.time + (1f / attackRate);
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

    //Apply class data when player selects a class in the ClassSelection scene
    public void ApplyClassData(CharacterClassData data)
    {
        if (data == null) return;

        // Assign ScriptableObject data to current session variables
        classData = data;
        moveSpeed = data.speed;
        jumpForce = data.jumpForce;
        dashForce = data.dashForce;
        stats.maxHealth = data.maxHealth;
        stats.currentHealth = stats.maxHealth; // Heal to full on class change
        isRangedClass = data.isRanged;
        attackDamage = data.damage;
        attackRange = data.attackRange;
        attackRate = data.attackRate;
        criticalChance = data.criticalChance;

        // Change visual feedback (placeholder color)
        spriteRenderer.color = data.classPreviewColor;
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0) return;

        stats.currentHealth -= damage; 
        invincibilityTimer = invincibilityDuration;

        if (stats.currentHealth <= 0) Die();
    }

    public float GetNextAttackTime() => nextAttackTime;

    public float GetNextSpecialAttackTime() => nextSpecialAttackTime;

    public float GetLastSpecialCooldownDuration() => lastSpecialCooldownDuration;  

    #endregion

    #region Private Logic

    //Initializes player stats based on the selected class at the start of the game
    private void InitializeStats()
    {
        if (CharacterClassData.SelectedClass != null)
            classData = CharacterClassData.SelectedClass;

        if (classData != null)
        {
            moveSpeed = classData.speed;
            jumpForce = classData.jumpForce;
            dashForce = classData.dashForce;

            stats.maxHealth = classData.maxHealth;
            stats.currentHealth = stats.maxHealth;

            isRangedClass = classData.isRanged;

            attackDamage = classData.damage;
            attackRange = classData.attackRange;
            attackRate = classData.attackRate;
            criticalChance = classData.criticalChance;

            spriteRenderer.color = classData.classPreviewColor;
        }
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
        //Look for interactable objects within range
        Collider2D[] foundObjects = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);

        IInteractable nearest = null;
        if (foundObjects.Length > 0)
        {
            //Choose the closest one (for simplicity, we take the first one found)
            nearest = foundObjects[0].GetComponent<IInteractable>();
        }

        //Handling the glow effect for the nearest interactable
        if (nearest != currentInteractable)
        {
            //Disbale old highlight
            if (currentInteractable != null) currentInteractable.SetHighlight(false);

            //SEt and enable new one
            currentInteractable = nearest;
            if (currentInteractable != null) currentInteractable.SetHighlight(true);
        }

        //The interaction input (E key) - only works if there's a valid interactable in range
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void HandleInvincibilityVisuals()
    {
        if (invincibilityTimer > 0)
        {
            if (invincibilityTimer > 1f) // If timer is long, it's probably "Rage"
            {
                // Intense red tint for Rage mode
                spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
            }
            else
            {
                // Standard flashing for short invincibility after hit
                float alpha = Mathf.Sin(Time.time * 20f) > 0 ? 1f : 0.2f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
        else
        {
            // Reset to class color
            if (classData != null)
                spriteRenderer.color = classData.classPreviewColor;
            else
                spriteRenderer.color = Color.white;
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
            MeleeAttack(false);
        }
    }

    private void Shoot(bool isExtraArrow = false)
    {
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        float direction = isFacingRight ? 1f : -1f;

        int totalDamage = attackDamage + stats.bonusDamage;
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * 2f); // Apply 100% bonus for critical hits
            Debug.Log("Critical hit!");
        }

        arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, 0f);
        if (!isFacingRight) arrow.transform.localScale = new Vector3(-1, 1, 1);

        // We should pass totalDamage to the Arrow script here if you have one
    }

    private void MeleeAttack(bool isSpecial = false)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            int totalDamage = attackDamage + stats.bonusDamage;
            bool isCritical = Random.value < criticalChance;
            if (isCritical)
            {
                totalDamage = Mathf.RoundToInt(totalDamage * 2f); // Apply 100% bonus for critical hits
                
                Debug.Log("Critical hit!");
            }

            if (isSpecial)
            {
                totalDamage *= 3; // Vanguard Strike is 3x stronger
                Debug.Log("Vanguard Strike hit for: " + totalDamage);
            }
            Debug.Log($"Attacking for {totalDamage} damage!");
            enemy.GetComponent<Enemy>().takedamage(totalDamage);
        }
    }

    private void MultiShot()
    {
        anim.SetTrigger("Attack"); // Reuse existing animation

        Debug.Log("Performing Multi-Shot!");

        // Shoot 3 arrows with slight vertical offsets/angles
        for (int i = -1; i <= 1; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            float direction = isFacingRight ? 1f : -1f;

            // Add vertical spread
            float verticalSpread = i * 2f;
            arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, verticalSpread);

            if (!isFacingRight) arrow.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void VanguardStrike()
    {
        // Trigger special animation if you have one, or reuse standard attack
        anim.SetTrigger("Attack");

        Debug.Log("Performing Vanguard Strike!");

        // Call melee attack with 'isSpecial' flag set to true
        MeleeAttack(true);
    }

    private void SpartanRage()
    {
        Debug.Log("Hercules enters SPARTAN RAGE!");

        // Use the existing invincibility system
        // Set the timer to the duration unlocked in PlayerStats
        invincibilityTimer = 5f;

        // Optional: Trigger an animation or sound effect here
        anim.SetTrigger("Attack");
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

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        // 1. Check if we collided with a coin
        if (collision.gameObject.CompareTag("Coin"))
        {
            // Accessing PlayerStats through the local reference 'stats' 
            // which we cached in the Start() method.
            if (stats != null)
            {
                stats.AddDenarii(5);
            }

            // Destroy the coin object after collection
            Destroy(collision.gameObject);
        }

        // 2. Check if we collided with a treasure
        if (collision.gameObject.CompareTag("Treasure"))
        {
            if (stats != null)
            {
                stats.AddDenarii(100);
            }

            // Destroy the treasure object
            Destroy(collision.gameObject);
        }
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