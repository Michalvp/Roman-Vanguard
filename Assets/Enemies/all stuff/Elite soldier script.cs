using UnityEngine;

public class Elitesoldierscript : Enemy
{

    // Start is called before the first frame update
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private Transform maxrightpoint;
    [SerializeField] private Transform maxleftpoint;
    [SerializeField] private Transform soldiermov;
    [SerializeField] private float shootingspeed = 10f;
    [SerializeField] private float size = 1;
    [SerializeField] private float range = 1;
    [SerializeField] private Animator anim;
    //[SerializeField] private float jumpcooldown = 5;
    private float timepassed = 0;
    [SerializeField] private Rigidbody2D enemy;
    void Start()
    {

    }
    private Vector3 startingscale;
    bool movingleft = true;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private BoxCollider2D boxCollider2;
    private void Awake()
    {
        startingscale = soldiermov.localScale;
    }
    int timepassed2 = 0;
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("cebula");
        }
        
    }*/
    // Update is called once per frame
    bool playerinmeleerange()
    {
        RaycastHit2D inshootingsight = Physics2D.BoxCast(boxCollider2.bounds.center +  range * transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z), 0, Vector2.left, 0, playermask);

        return inshootingsight.collider != null;

    }
    bool playerinsight()
    {
        RaycastHit2D inshootingsight = Physics2D.BoxCast(boxCollider2.bounds.center +  range * transform.right*2 * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x*5 * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z), 0, Vector2.left, 0, playermask);

        return inshootingsight.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider2.bounds.center + transform.right * transform.localScale.x*range, new Vector3(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCollider2.bounds.center + transform.right * transform.localScale.x*2 *range, new Vector3(boxCollider2.bounds.size.x * size*5, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z));
    }
    private void Move(int direction)
    {
        soldiermov.position = new Vector3(soldiermov.position.x + movementspeed * Time.deltaTime * direction, soldiermov.position.y, soldiermov.position.z);
        soldiermov.localScale = new Vector3(Mathf.Abs(startingscale.x) * direction, startingscale.y, startingscale.z);
    }
    public bool ifhitwall()
    {
        RaycastHit2D collidingwall = Physics2D.BoxCast(boxCollider2.bounds.center + 5 * transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * 20, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z), 0, Vector2.left, 5, playermask);
        return collidingwall.collider != null;
    }
    void Update()
    {
        // Debug.Log("troll");
        timepassed2++;

        {
            if (movingleft)
            {
                if (soldiermov.position.x >= maxleftpoint.position.x)
                { Move(-1); }
                else
                {
                    movementspeed = 5f;
                    movingleft = !movingleft;

                }
            }
            else
            {
                if (soldiermov.position.x <= maxrightpoint.position.x)
                { Move(1); }
                else
                { movingleft = !movingleft;
                    movementspeed = 5f;
                }
            }

            timepassed++;
            if (playerinsight())
            {
                movementspeed = 10f;
            }
                if (playerinmeleerange())
            {
                if (timepassed * Time.deltaTime >= shootingspeed)
                {
                    timepassed = 0;
                    player.GetComponent<PlayerController>().TakeDamage(damage);
                    // player takes damage
                }
            }
        }
    }
}
