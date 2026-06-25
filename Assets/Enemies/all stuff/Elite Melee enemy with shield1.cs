using UnityEngine;

public class EliteMeleeenemywithshield : Enemy
{
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private Transform maxrightpoint;
    [SerializeField] private Transform maxleftpoint;
    [SerializeField] private Transform soldiermov;
    [SerializeField] private float shootingspeed = 10f;
    [SerializeField] private float size = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject weapon;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private BoxCollider2D boxCollider2;

    private float timepassed;
    private bool attacking;
    private Vector3 startingscale;
    private bool movingleft = true;
    private Spear spear;

    private void Awake()
    {
        if (soldiermov != null)
        {
            startingscale = soldiermov.localScale;
        }

        if (weapon != null)
        {
            spear = weapon.GetComponent<Spear>();
        }
    }

    private void Start()
    {
        if (spear != null)
        {
            spear.setdamage(damage);
        }

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
            size * range * transform.right * transform.localScale.x,
            new Vector3(
                boxCollider2.bounds.size.x * range,
                boxCollider2.bounds.size.y,
                boxCollider2.bounds.size.z
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

    public bool ifhitwall()
    {
        if (boxCollider2 == null)
        {
            return false;
        }

        RaycastHit2D collidingwall = Physics2D.BoxCast(
            boxCollider2.bounds.center +
            5f * transform.right * transform.localScale.x,
            new Vector3(
                boxCollider2.bounds.size.x * 20f,
                boxCollider2.bounds.size.y,
                boxCollider2.bounds.size.z
            ),
            0f,
            Vector2.left,
            5f,
            playermask
        );

        return collidingwall.collider != null;
    }

    private void Update()
    {
        Patrol();

        timepassed += Time.deltaTime;
        attacking = playerinsight();

        if (attacking)
        {
            BeginSpearAttack();
        }
    }

    private void BeginSpearAttack()
    {
        if (spear == null)
        {
            return;
        }

        spear.direction = movingleft ? -1 : 1;

        if (!spear.attack && !spear.returning)
        {
            AudioManager.Instance?.PlayEnemyAttack(
                EnemyAttackSoundGroup.Melee
            );

            spear.attack = true;
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

                if (spear != null)
                {
                    spear.direction = -1;
                }
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

                if (spear != null)
                {
                    spear.direction = 1;
                }
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
            transform.right * transform.localScale.x,
            new Vector3(
                boxCollider2.bounds.size.x * range,
                boxCollider2.bounds.size.y,
                boxCollider2.bounds.size.z
            )
        );
    }
}
