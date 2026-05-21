using UnityEngine;

public class Meleeenemyscript : Enemy
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
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private Vector3 startingscale;
    bool movingleft = true;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private BoxCollider2D boxCollider2;
    private void Awake()
    {
        startingscale = soldiermov.localScale;
    }
    bool playerinsight()
    {
        RaycastHit2D inshootingsight = Physics2D.BoxCast(boxCollider2.bounds.center +  range * transform.right * transform.localScale.x, new Vector2(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y), 0, Vector2.left, 0, playermask);

        return inshootingsight.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider2.bounds.center +  range *transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z));
    }
    private void Move(int direction)
    {
        soldiermov.position = new Vector3(soldiermov.position.x + movementspeed * Time.deltaTime * direction, soldiermov.position.y, soldiermov.position.z);
        soldiermov.localScale = new Vector3(Mathf.Abs(startingscale.x) * direction, startingscale.y, startingscale.z);
    }
    void Update()
    {
        
            if (movingleft)
            {
                if (soldiermov.position.x >= maxleftpoint.position.x)
                { Move(-1); }
                else
                {

                    movingleft = !movingleft;

                }
            }
            else
            {
                if (soldiermov.position.x <= maxrightpoint.position.x)
                { Move(1); }
                else
                { movingleft = !movingleft; }
            }

            timepassed++;

            if (playerinsight())
            {
            Debug.Log("Player in sight");
            if (timepassed *Time.deltaTime>= shootingspeed)
                {
                    timepassed = 0;
                    player.GetComponentInChildren<PlayerController>().TakeDamage(damage);
                }
            }
        
    }
}
