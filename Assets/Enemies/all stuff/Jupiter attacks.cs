using UnityEngine;

public class Jupiterattacks : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject Lightning;
    [SerializeField] private Transform Player;

    private float timepassed;
    private int randomnumber = 40;
    private short attack;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed <= randomnumber)
        {
            return;
        }

        timepassed = 0f;
        attack = (short)UnityEngine.Random.Range(0, 2);
        randomnumber = UnityEngine.Random.Range(20, 40);

        // One boss sound for one newly released boss attack.
        AudioManager.Instance?.PlayEnemyAttack(
            EnemyAttackSoundGroup.Boss
        );

        if (attack == 0)
        {
            CreateBoltAttack();
        }
        else
        {
            CreateLightningAttack();
        }
    }

    private void CreateBoltAttack()
    {
        if (bullet == null)
        {
            return;
        }

        GameObject currentBullet = Instantiate(bullet, transform);
        currentBullet.transform.localScale =
            new Vector3(0.5f, 5f, 1f);

        currentBullet.transform.position = new Vector3(
            transform.position.x -
            UnityEngine.Random.Range(0, 30) -
            30f,
            transform.position.y,
            0f
        );

        if (currentBullet.GetComponent<Collider2D>() != null)
        {
            Destroy(currentBullet, 3f);
        }
    }

    private void CreateLightningAttack()
    {
        if (Lightning == null)
        {
            return;
        }

        GameObject lightning = Instantiate(Lightning, transform);
        lightning.transform.position =
            new Vector3(transform.position.x, transform.position.y, 0f);

        lightning.transform.localScale =
            new Vector3(0.1f, 0.1f, 0f);

        if (lightning.GetComponent<Collider2D>() != null)
        {
            Destroy(lightning, 3f);
        }
    }
}
