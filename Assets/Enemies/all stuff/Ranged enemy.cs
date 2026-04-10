using UnityEngine;

public class Rangedenemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private float shootingspeed = 5f;
    [SerializeField] private float size = 10;
    [SerializeField] private float range = 10;
    [SerializeField] private Animator anim;
    private float timepassed = 0;
    [SerializeField] private LayerMask playermask;
    [SerializeField] private Transform projstart;
    //[SerializeField] private GameObject[] bullet;
    [SerializeField] private BoxCollider2D boxCollider2;
    // Start is called before the first frame update
    void Start()
    {

    }
    [SerializeField] private Transform maxrightpoint;
    [SerializeField] private Transform maxleftpoint;
    [SerializeField] private Transform soldiermov;
    private Vector3 startingscale;
    [SerializeField] private GameObject bullet;
    bool movingleft = true;
    private void Awake()
    {
        startingscale = soldiermov.localScale;
    }
    // Update is called once per frame
    public void rangedattack(int direction)
    {
        GameObject currentBullet = Instantiate(bullet, transform);
        currentBullet.transform.position = transform.position;
        currentBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * -20, 0), ForceMode2D.Impulse);
        if (currentBullet.GetComponent<Collider2D>())
            Destroy(currentBullet, 3);
        
    }


    /* private int findbullet()
     {
         for (int i=0; i < bullet.Length; i++)
         {
             if (!bullet[i].activeInHierarchy)
             {
                 return i;
             }
         }
         return 0;
     }*/
    bool playerinsight()
    {
        RaycastHit2D inshootingsight = Physics2D.BoxCast(boxCollider2.bounds.center  + range  * transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z), 0, Vector2.left, 0, playermask);

        return inshootingsight.collider != null;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider2.bounds.center  + range * transform.right * transform.localScale.x, new Vector3(boxCollider2.bounds.size.x * size, boxCollider2.bounds.size.y, boxCollider2.bounds.size.z));
    }
    private void Move(int direction)
    {
        soldiermov.position = new Vector3(soldiermov.position.x + movementspeed * Time.deltaTime * direction, soldiermov.position.y, soldiermov.position.z);
        soldiermov.localScale = new Vector3(Mathf.Abs(startingscale.x) * direction, startingscale.y, startingscale.z);
    }
    public void die()
    {
        anim.SetTrigger("Die");
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
            if (timepassed * Time.deltaTime >= shootingspeed)
            {
                if (movingleft)
                    rangedattack(1);
                else
                    rangedattack(-1);
                timepassed = 0;
            }
        }
    }
}
