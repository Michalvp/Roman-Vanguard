using UnityEngine;

public class Neptunattacks : MonoBehaviour
{
    [SerializeField] private GameObject Waterburst;
    [SerializeField] private GameObject Water;
    [SerializeField] private int HP;
    [SerializeField] private int MaxHP;

    private float timepassed;
    private short randomnumber = 40;
    private short attack;
    private short attackcooldown = 10;
    private int watercooldown;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed <= randomnumber)
        {
            return;
        }

        if (watercooldown == 0)
        {
            attack = (short)UnityEngine.Random.Range(0, 2);
        }
        else
        {
            attack = 0;
            watercooldown -= randomnumber;
        }

        randomnumber = (short)UnityEngine.Random.Range(
            10 + attackcooldown,
            30 + attackcooldown
        );

        if (MaxHP * 0.5f > HP)
        {
            attackcooldown = 0;
        }

        AudioManager.Instance?.PlayEnemyAttack(
            EnemyAttackSoundGroup.Boss
        );

        if (attack == 0)
        {
            CreateWaterBurst();
        }
        else
        {
            CreateRisingWater();
        }

        timepassed = 0f;
    }

    private void CreateWaterBurst()
    {
        if (Waterburst == null)
        {
            return;
        }

        Debug.Log("waterburst");

        GameObject waterBurst =
            Instantiate(Waterburst, transform);

        waterBurst.transform.position = new Vector3(
            transform.position.x - 5f,
            transform.position.y + 3f,
            transform.position.z
        );

        waterBurst.transform.localScale =
            new Vector3(0.1f, 0.1f, 0f);

        if (waterBurst.GetComponent<Collider2D>() != null)
        {
            Destroy(waterBurst, 3f);
        }
    }

    private void CreateRisingWater()
    {
        if (Water == null)
        {
            return;
        }

        watercooldown = 50;

        GameObject water = Instantiate(Water, transform);
        water.transform.localScale =
            new Vector3(20f, 0.5f, 1f);

        water.transform.position = new Vector3(
            transform.position.x - 50f,
            transform.position.y - 15f,
            transform.position.z
        );
    }
}
