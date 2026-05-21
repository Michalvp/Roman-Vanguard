using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    private int damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * 20f;
    }
    public void setdamage(int damage)
    {
        this.damage = damage;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
        }
    }
}
