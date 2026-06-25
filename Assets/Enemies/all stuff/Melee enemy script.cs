using UnityEngine;

public class Meleeenemyscript : Enemy
{
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private Transform maxrightpoint;
    [SerializeField] private Transform maxleftpoint;
    [SerializeField] private Transform soldiermov;
    [SerializeField] private float shootingspeed = 10f;
    [SerializeField] private float size = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D enemy;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private BoxCollider2D boxCollider2;

    private float timepassed;
    private Vector3 startingscale;
    private bool movingleft = true;

    private void Awake()
    {
        if (soldiermov != null)
        {
            startingscale = soldiermov.localScale;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private bool playerinsight()
    {
        if (boxCollider2 == null)
        {
            return false;
        }

        RaycastHit2D inshootingsight = Physics2D.BoxCast(
            boxCollider2.bounds.center +
            range * transform.right * transform.localScale.x,
            new Vector2(
                boxCollider2.bounds.size.x * size,
                boxCollider2.bounds.size.y
            ),
            0f,
            Vector2.left,
            0f,
            playermask
        );

        return inshootingsight.collider != null;
    }

    private void Move(int direction)
    {
        if (soldiermov == null)
        {
            return;
        }

        soldiermov.position = new Vector3(
            soldiermov.position.x +
            movementspeed * Time.deltaTime * direction,
            soldiermov.position.y,
            soldiermov.position.z
        );

        soldiermov.localScale = new Vector3(
            Mathf.Abs(startingscale.x) * direction,
            startingscale.y,
            startingscale.z
        );
    }

    private void Update()
    {
        Patrol();

        timepassed += Time.deltaTime;

        if (playerinsight() && timepassed >= shootingspeed)
        {
            timepassed = 0f;

            PlayerController playerController =
                player != null
                    ? player.GetComponentInChildren<PlayerController>()
                    : null;

            if (playerController != null)
            {
                // PDF guide proposal:
                // Play the melee attack sound immediately before damage lands.
                AudioManager.Instance?.PlayEnemyAttack(
                    EnemyAttackSoundGroup.Melee
                );

                playerController.TakeDamage(damage);
            }
        }
    }

    private void Patrol()
    {
        if (soldiermov == null ||
            maxleftpoint == null ||
            maxrightpoint == null)
        {
            return;
        }

        if (movingleft)
        {
            if (soldiermov.position.x >= maxleftpoint.position.x)
            {
                Move(-1);
            }
            else
            {
                movingleft = false;
            }
        }
        else
        {
            if (soldiermov.position.x <= maxrightpoint.position.x)
            {
                Move(1);
            }
            else
            {
                movingleft = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider2 == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider2.bounds.center +
            range * transform.right * transform.localScale.x,
            new Vector3(
                boxCollider2.bounds.size.x * size,
                boxCollider2.bounds.size.y,
                boxCollider2.bounds.size.z
            )
        );
    }
}
