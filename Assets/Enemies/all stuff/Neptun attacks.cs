using Unity.Mathematics;
using UnityEngine;

public class Neptunattacks : MonoBehaviour
{
    private int timepassed = 0;
    private short randomnumber = 40;
    private short attack = 0;
    private short attackcooldown = 10;
    [SerializeField] private GameObject Waterburst;
    [SerializeField] private GameObject Water;
    [SerializeField] private int HP;
    [SerializeField] private int MaxHP;
    [SerializeField] private Transform Player;
    private int watercooldown=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timepassed++;
        if (timepassed * Time.deltaTime > randomnumber)
        {
            if (watercooldown ==0)
            attack = (short)UnityEngine.Random.Range(0, 2);
            else
            {
                attack = 0;
                watercooldown -= randomnumber;
            }
            randomnumber = (short)UnityEngine.Random.Range(10 + attackcooldown, 30 + attackcooldown);
            if (MaxHP * 0.5f > HP)
            {
                attackcooldown = 0;
            }
            if (attack == 0)
            {
                GameObject lightning = Instantiate(Waterburst, transform);
                lightning.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                lightning.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                lightning.transform.rotation = new Quaternion(math.tan(math.abs(Player.position.x - transform.position.x) / math.abs(Player.position.y - transform.position.y)), transform.rotation.y, transform.rotation.z, transform.rotation.w);
                if (lightning.GetComponent<Collider2D>())
                    Destroy(lightning, 3);
            }
            else
            {
                watercooldown = 50;
                GameObject water = Instantiate(Water, transform);
                water.transform.localScale = new Vector3(20, 0.5f, 1);
                water.transform.position = new Vector3(transform.position.x - 50, transform.position.y - 30, transform.position.z);
            }
            timepassed = 0;
        }
    }
}
