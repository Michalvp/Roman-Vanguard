using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Settings

    [Header("Class Configuration")]
    public CharacterClassData classData;

    [Header("Movement & Physicality")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;
    public float dashForce = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Detection & Environment")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float interactionRange = 2f;
    public LayerMask interactableLayer;
    public KeyCode interactionKey = KeyCode.S;

    [Header("Base RPG Stats")]
    public float invincibilityDuration = 0.5f;
    public float attackRate = 2f;
    public float criticalChance = 0.1f;

    [Header("Combat - Melee")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public LayerMask enemyLayers;

    [Header("Combat - Ranged")]
    public bool isRangedClass = false;
    public GameObject arrowPrefab;
    public float arrowSpeed = 20f;

    #endregion

    #region Private State

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isFacingRight = true;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private IInteractable currentInteractable;
    private PlayerStats stats;
    private float invincibilityTimer;
    private float nextAttackTime = 0f;
    private float nextSpecialAttackTime = 0f;
    private float lastSpecialCooldownDuration = 1f;
    private bool canDash = true;
    private bool isDashing;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = GetComponent<PlayerStats>();

        if (stats == null)
            stats = gameObject.AddComponent<PlayerStats>();

        if (GetComponent<PlayerInventory>() == null)
            gameObject.AddComponent<PlayerInventory>();

        InitializeStats();
    }

    private void Update()
    {
        // Menus should stop player movement/combat.
        // Exception: if shop is open, we still allow HandleInteraction so S can close it.
        if (GameUIState.IsAnyBlockingMenuOpen)
        {
            StopHorizontalMovement();
            UpdateAnimations();

            if (GameUIState.IsShopOpen)
                HandleInteraction();

            if (invincibilityTimer > 0)
                invincibilityTimer -= Time.deltaTime;

            HandleInvincibilityVisuals();
            return;
        }

        UpdateAnimations();

        if (isDashing)
            return;

        HandleInput();
        HandleJump();
        HandleInteraction();

        if (Time.time >= nextAttackTime && Input.GetButtonDown("Fire1"))
        {
            Attack();
            nextAttackTime = Time.time + 1f / GetTotalAttackRate();
        }

        if (Input.GetButtonDown("Fire2") && stats.hasSpecialAbility && Time.time >= nextSpecialAttackTime)
        {
            UseSpecialAttack();
            nextAttackTime = Time.time + 1f / GetTotalAttackRate();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        if (invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;

        HandleInvincibilityVisuals();
    }

    private void FixedUpdate()
    {
        if (GameUIState.IsAnyBlockingMenuOpen)
        {
            StopHorizontalMovement();
            CheckGround();
            return;
        }

        if (isDashing)
            return;

        HandleMovement();
        CheckGround();
    }

    #endregion

    #region Public API

    public void ApplyClassData(CharacterClassData data)
    {
        if (data == null)
            return;

        classData = data;
        CharacterClassData.SelectedClass = data;

        moveSpeed = data.speed;
        jumpForce = data.jumpForce;
        dashForce = data.dashForce;

        isRangedClass = data.isRanged;
        attackDamage = data.damage;
        attackRange = data.attackRange;
        attackRate = data.attackRate;
        criticalChance = data.criticalChance;

        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (stats != null)
            stats.SetClassBaseStats(data);

        if (spriteRenderer != null)
            spriteRenderer.color = data.classPreviewColor;
    }

    public void TakeDamage(int damage)
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (stats == null)
            return;

        if (invincibilityTimer > 0)
            return;

        int reducedDamage = Mathf.Max(1, damage - stats.bonusDefense);
        stats.currentHealth -= reducedDamage;

        Debug.Log($"Player took {reducedDamage} damage after armor reduction.");

        invincibilityTimer = invincibilityDuration;

        if (stats.currentHealth <= 0)
            Die();
    }

    public float GetNextAttackTime() => nextAttackTime;
    public float GetNextSpecialAttackTime() => nextSpecialAttackTime;
    public float GetLastSpecialCooldownDuration() => lastSpecialCooldownDuration;

    public float GetTotalMoveSpeed()
    {
        return Mathf.Max(1f, moveSpeed + (stats != null ? stats.bonusMoveSpeed : 0f));
    }

    public int GetTotalAttackDamage()
    {
        return Mathf.Max(1, attackDamage + (stats != null ? stats.bonusDamage : 0));
    }

    public float GetTotalAttackRate()
    {
        return Mathf.Max(0.2f, attackRate + (stats != null ? stats.bonusAttackRate : 0f));
    }

    public float GetTotalAttackRange()
    {
        return Mathf.Max(0.1f, attackRange + (stats != null ? stats.bonusAttackRange : 0f));
    }

    public float GetTotalCriticalChance()
    {
        return Mathf.Clamp01(criticalChance + (stats != null ? stats.bonusCriticalChance : 0f));
    }

    #endregion

    #region Private Logic

    private void InitializeStats()
    {
        if (CharacterClassData.SelectedClass != null)
            classData = CharacterClassData.SelectedClass;

        if (classData != null)
            ApplyClassData(classData);
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !isFacingRight)
            Flip();
        else if (moveInput < 0 && isFacingRight)
            Flip();
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void HandleInteraction()
    {
        Collider2D[] foundObjects = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        IInteractable nearest = null;

        if (foundObjects.Length > 0)
        {
            float bestDistance = float.MaxValue;

            foreach (Collider2D foundObject in foundObjects)
            {
                IInteractable interactable = foundObject.GetComponent<IInteractable>();

                if (interactable == null)
                    interactable = foundObject.GetComponentInParent<IInteractable>();

                if (interactable == null)
                    continue;

                float distance = Vector2.Distance(transform.position, foundObject.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = interactable;
                }
            }
        }

        if (nearest != currentInteractable)
        {
            if (currentInteractable != null)
                currentInteractable.SetHighlight(false);

            currentInteractable = nearest;

            if (currentInteractable != null)
                currentInteractable.SetHighlight(true);
        }

        if (Input.GetKeyDown(interactionKey) && currentInteractable != null)
            currentInteractable.Interact();
    }

    private void HandleInvincibilityVisuals()
    {
        if (spriteRenderer == null)
            return;

        if (invincibilityTimer > 0)
        {
            if (invincibilityTimer > 1f)
            {
                spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 1f);
            }
            else
            {
                float alpha = Mathf.Sin(Time.time * 20f) > 0 ? 1f : 0.2f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
        else
        {
            spriteRenderer.color = classData != null ? classData.classPreviewColor : Color.white;
        }
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput * GetTotalMoveSpeed(), rb.linearVelocity.y);
    }

    private void StopHorizontalMovement()
    {
        moveInput = 0f;

        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        if (groundCheck == null)
            return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Attack()
    {
        if (anim != null)
            anim.SetTrigger("Attack");

        if (isRangedClass)
            Shoot();
        else
            MeleeAttack(false);
    }

    private void Shoot(bool isExtraArrow = false)
    {
        if (arrowPrefab == null || attackPoint == null)
            return;

        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        float direction = isFacingRight ? 1f : -1f;
        int totalDamage = CalculateDamageWithCritical();

        if (arrowRb != null)
            arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, 0f);

        if (!isFacingRight)
            arrow.transform.localScale = new Vector3(-1, 1, 1);

        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript != null)
            arrowScript.setdamage(totalDamage);
    }

    private void MeleeAttack(bool isSpecial = false)
    {
        if (attackPoint == null)
            return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, GetTotalAttackRange(), enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            int totalDamage = CalculateDamageWithCritical();

            if (isSpecial)
                totalDamage *= 3;

            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if (enemyScript != null)
                enemyScript.takedamage(totalDamage);

            Debug.Log($"Attacking for {totalDamage} damage!");
        }
    }

    private int CalculateDamageWithCritical()
    {
        int totalDamage = GetTotalAttackDamage();

        bool isCritical = Random.value < GetTotalCriticalChance();

        if (isCritical)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * 2f);
            Debug.Log("Critical hit!");
        }

        return totalDamage;
    }

    private void UseSpecialAttack()
    {
        if (classData == null)
            return;

        if (classData.className == "Archer")
        {
            lastSpecialCooldownDuration = (1f / GetTotalAttackRate()) * 1.5f;
            nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration;
            MultiShot();
        }
        else if (classData.className == "Legionary")
        {
            lastSpecialCooldownDuration = (1f / GetTotalAttackRate()) * 2f;
            nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration;
            VanguardStrike();
        }
        else if (classData.className == "Gladiator")
        {
            lastSpecialCooldownDuration = 15f;
            nextSpecialAttackTime = Time.time + lastSpecialCooldownDuration;
            SpartanRage();
        }
    }

    private void MultiShot()
    {
        if (anim != null)
            anim.SetTrigger("Attack");

        if (arrowPrefab == null || attackPoint == null)
            return;

        for (int i = -1; i <= 1; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, Quaternion.identity);
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

            float direction = isFacingRight ? 1f : -1f;
            float verticalSpread = i * 2f;

            if (arrowRb != null)
                arrowRb.linearVelocity = new Vector2(direction * arrowSpeed, verticalSpread);

            if (!isFacingRight)
                arrow.transform.localScale = new Vector3(-1, 1, 1);

            Arrow arrowScript = arrow.GetComponent<Arrow>();

            if (arrowScript != null)
                arrowScript.setdamage(CalculateDamageWithCritical());
        }
    }

    private void VanguardStrike()
    {
        if (anim != null)
            anim.SetTrigger("Attack");

        MeleeAttack(true);
    }

    private void SpartanRage()
    {
        invincibilityTimer = 5f;

        if (anim != null)
            anim.SetTrigger("Attack");
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        invincibilityTimer = dashDuration;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Die()
    {
        Debug.Log("Ave Caesar, morituri te salutant! The player died.");
        enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void UpdateAnimations()
    {
        if (anim == null || rb == null)
            return;

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isDashing", isDashing);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (stats == null)
            return;

        if (collision.gameObject.CompareTag("Coin"))
        {
            stats.AddDenarii(5);
            Debug.Log("Collected a coin! Total denarii: " + stats.denarii);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Treasure"))
        {
            stats.AddDenarii(100);
            Debug.Log("Collected treasure! Total denarii: " + stats.denarii);
            Destroy(collision.gameObject);
        }
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    #endregion
}
