using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    private int damage;
    private string shooter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction;
        if (shooter == "enemy")
        {
            direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * 20f;
        }
       /* else if (shooter == "player")
        {
        if (player.GetComponent<Transform>().localScale.x > 0f)
            direction = transform.position + Vector3.right * 20f;
        else
            direction = transform.position + Vector3.left * 20f;
        rb.linearVelocity = direction;
        }*/
        }
    public void setdamage(int damage, string shooter)
    {
        this.damage = damage;
        this.shooter = shooter;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && shooter == "enemy")
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 8 && shooter == "player")
        {
            Debug.Log("shoted enemy");
            collision.gameObject.GetComponent<Enemy>().takedamage(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
        }
    }
}
