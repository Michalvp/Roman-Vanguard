using UnityEngine;

public class Boltscript : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (rb == null || player == null)
        {
            return;
        }

        Vector3 direction =
            (player.transform.position - transform.position).normalized;

        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Wall"))
        {
            // Jupiterattacks already plays the boss sound when the attack
            // starts, so this projectile does not play it again.
            Destroy(gameObject);
        }
    }
}
