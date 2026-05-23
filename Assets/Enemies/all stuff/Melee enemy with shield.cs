using UnityEngine;

public class Meleeenemywithshield : Enemy
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
    [SerializeField] private GameObject weapon;
    private float timepassed = 0;
    private bool attacking = false;
    void Start()
    {
        weapon.GetComponent<Spear>().setdamage(damage);
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
    int timepassed2 = 0;
    bool playerinsight()
    {
        RaycastHit2D inshootingsight = Physics2D.BoxCast(boxCollider2.bounds.center + size * range * transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * range, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z), 0, Vector2.left, 0, playermask);

        return inshootingsight.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider2.bounds.center + transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * range, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z));
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
        timepassed2++;

        {
            if(attacking == false)
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
            }

            timepassed++;

            if (playerinsight())
            {
                attacking = true;
                weapon.GetComponent<Spear>().attack = true;
                if(movingleft)
                {
                    weapon.GetComponent<Spear>().direction = -1;
                }
                else
                {
                    weapon.GetComponent<Spear>().direction = 1;
                }
            }
            else
            {
                if (weapon.GetComponent<Spear>().returning == false && weapon.GetComponent<Spear>().attack==false)
                {
                    attacking = false;
                }

            }
        }
    }
}
