using UnityEngine;

public class EliteArcher : Enemy
{
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private float shootingspeed = 5f;
    [SerializeField] private float size = 10f;
    [SerializeField] private float range = 10f;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private Transform projstart;
    [SerializeField] private BoxCollider2D boxCollider2;
    [SerializeField] private Transform maxrightpoint;
    [SerializeField] private Transform maxleftpoint;
    [SerializeField] private Transform soldiermov;
    [SerializeField] private GameObject bullet;

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

    public void rangedattack(int direction)
    {
        if (bullet == null)
        {
            return;
        }

        AudioManager.Instance?.PlayEnemyAttack(
            EnemyAttackSoundGroup.Ranged
        );

        GameObject currentBullet = Instantiate(bullet, transform);

        if (projstart != null)
        {
            currentBullet.transform.position = projstart.position;
        }

        Arrow arrow = currentBullet.GetComponent<Arrow>();

        if (arrow != null)
        {
            arrow.setdamage(damage);
        }

        if (currentBullet.GetComponent<Collider2D>() != null)
        {
            Destroy(currentBullet, 3f);
        }
    }

    private bool playerinsight()
    {
        if (boxCollider2 == null)
        {
            return false;
        }

        RaycastHit2D inshootingsight = Physics2D.BoxCast(
            boxCollider2.bounds.center +
            range * transform.right * transform.localScale.x +
            transform.up * transform.localScale.y * 2f,
            new Vector3(
                boxCollider2.bounds.size.x * size,
                boxCollider2.bounds.size.y + 3f,
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

    private void Update()
    {
        Patrol();

        timepassed += Time.deltaTime;

        if (playerinsight() && timepassed >= shootingspeed)
        {
            rangedattack(movingleft ? 1 : -1);
            timepassed = 0f;
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
            range * transform.localScale.x * transform.right +
            transform.up * transform.localScale.y * 2f,
            new Vector3(
                boxCollider2.bounds.size.x * size,
                boxCollider2.bounds.size.y + 3f,
                boxCollider2.bounds.size.z
            )
        );
    }
}
