using System;
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
                Debug.Log("waterburst");
                GameObject lightning = Instantiate(Waterburst, transform);
                lightning.transform.position = new Vector3(transform.position.x - 5, transform.position.y+3, transform.position.z);
                lightning.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                if (lightning.GetComponent<Collider2D>())
                    Destroy(lightning, 3);
            }
            else
            {
                watercooldown = 50;
                GameObject water = Instantiate(Water, transform);
                water.transform.localScale = new Vector3(20, 0.5f, 1);
                water.transform.position = new Vector3(transform.position.x - 50, transform.position.y - 15, transform.position.z);
            }
            timepassed = 0;
        }
    }
}
