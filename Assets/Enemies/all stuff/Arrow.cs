using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private Rigidbody2D rb;
    private GameObject player;
    private int damage;

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

    public void setdamage(int damage)
    {
        this.damage = damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController =
                collision.gameObject.GetComponent<PlayerController>();

            if (playerController == null)
            {
                playerController =
                    collision.gameObject
                        .GetComponentInChildren<PlayerController>();
            }

            if (playerController != null)
            {
                // PlayerController plays the hurt/death sound.
                // Do not play the ranged attack sound here because the
                // enemy already played it when this arrow was launched.
                playerController.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
        }
    }
}
