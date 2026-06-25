using UnityEngine;

public class Spear : MonoBehaviour
{
    private GameObject player;

    public bool attack = false;
    public bool returning = false;
    public int direction = 1;

    private int attackprogress;
    private float timepassed;
    private int damage;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (!attack || timepassed <= 0.1f)
        {
            return;
        }

        if (attackprogress < 20 && !returning)
        {
            attackprogress++;

            transform.position = new Vector3(
                transform.position.x + 0.1f * direction,
                transform.position.y,
                transform.position.z
            );
        }
        else
        {
            returning = true;
            attackprogress--;

            transform.position = new Vector3(
                transform.position.x - 0.1f * direction,
                transform.position.y,
                transform.position.z
            );

            if (attackprogress <= 0)
            {
                attackprogress = 0;
                attack = false;
                returning = false;
            }
        }

        timepassed = 0f;
    }

    public void setdamage(int damage)
    {
        this.damage = damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

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
            // The owning shield enemy plays the melee attack sound
            // when this spear begins moving. PlayerController plays hurt.
            playerController.TakeDamage(damage);
        }
    }
}
