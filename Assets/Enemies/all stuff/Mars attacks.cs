using UnityEngine;

public class Marsattacks : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject Sword;
    [SerializeField] private int HP;
    [SerializeField] private int MaxHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private int timepassed=0;
    private short randomnumber = 40;
    private short attack = 0;
    private short amountofarrows= 5;
    private short attackcooldown = 10;
    // Update is called once per frame
    void Update()
    {

        timepassed++;
        if (timepassed * Time.deltaTime > randomnumber)
        {
            randomnumber = (short)UnityEngine.Random.Range(10+attackcooldown, 30+attackcooldown);
            attack= (short)UnityEngine.Random.Range(0, 2);
            if (MaxHP * 0.5f > HP)
            {
                attackcooldown = 0;
                amountofarrows = 10;
            }
            if (attack == 0)
            {
            for (int i = 0; i < amountofarrows; i++)
            {
                GameObject currentBullet = Instantiate(bullet, transform);
                currentBullet.transform.localScale = new Vector3(0.1f,0.1f,1);
                currentBullet.transform.position = new Vector3(transform.position.x+10, transform.position.y + UnityEngine.Random.Range(-10,10), transform.position.z);
                currentBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-20 + UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(0, 10)), ForceMode2D.Impulse);
                if (currentBullet.GetComponent<Collider2D>())
                    Destroy(currentBullet, 3);
            }
            }
            else
            {
                GameObject sword = Instantiate(Sword, transform);
                sword.transform.localScale = new Vector3(1, 0.1f, 1);
                sword.transform.position = new Vector3(transform.position.x-5, transform.position.y+10, transform.position.z);
            }
            timepassed = 0;
        }
    }
}
