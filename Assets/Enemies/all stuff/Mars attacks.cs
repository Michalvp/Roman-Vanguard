using UnityEngine;

public class Marsattacks : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject Sword;
    [SerializeField] private int HP;
    [SerializeField] private int MaxHP;

    private float timepassed;
    private short randomnumber = 40;
    private short attack;
    private short amountofarrows = 5;
    private short attackcooldown = 10;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed <= randomnumber)
        {
            return;
        }

        randomnumber = (short)UnityEngine.Random.Range(
            10 + attackcooldown,
            30 + attackcooldown
        );

        attack = (short)UnityEngine.Random.Range(0, 2);

        if (MaxHP * 0.5f > HP)
        {
            attackcooldown = 0;
            amountofarrows = 10;
        }

        AudioManager.Instance?.PlayEnemyAttack(
            EnemyAttackSoundGroup.Boss
        );

        if (attack == 0)
        {
            CreateArrowVolley();
        }
        else
        {
            CreateFallingSword();
        }

        timepassed = 0f;
    }

    private void CreateArrowVolley()
    {
        if (bullet == null)
        {
            return;
        }

        for (int i = 0; i < amountofarrows; i++)
        {
            GameObject currentBullet =
                Instantiate(bullet, transform);

            currentBullet.transform.localScale =
                new Vector3(0.1f, 0.1f, 1f);

            currentBullet.transform.position = new Vector3(
                transform.position.x + 10f,
                transform.position.y +
                UnityEngine.Random.Range(-10, 10),
                transform.position.z
            );

            Rigidbody2D bulletBody =
                currentBullet.GetComponent<Rigidbody2D>();

            if (bulletBody != null)
            {
                bulletBody.AddForce(
                    new Vector2(
                        -20f + UnityEngine.Random.Range(-10, 10),
                        UnityEngine.Random.Range(0, 10)
                    ),
                    ForceMode2D.Impulse
                );
            }

            if (currentBullet.GetComponent<Collider2D>() != null)
            {
                Destroy(currentBullet, 3f);
            }
        }
    }

    private void CreateFallingSword()
    {
        if (Sword == null)
        {
            return;
        }

        GameObject sword = Instantiate(Sword, transform);
        sword.transform.localScale =
            new Vector3(1f, 0.1f, 1f);

        sword.transform.position = new Vector3(
            transform.position.x - 5f,
            transform.position.y + 10f,
            transform.position.z
        );
    }
}
