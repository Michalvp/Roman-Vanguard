using System;
using Unity.Mathematics;
using UnityEngine;
public class Jupiterattacks : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject Lightning;
    [SerializeField] private Transform Player;
    private int timepassed = 0;
    private int randomnumber = 40;
    private short attack = 0;
    // Update is called once per frame
    void Update()
    {
        timepassed++;
        if (timepassed * Time.deltaTime > randomnumber)
        {
            timepassed = 0;
            attack = (short)UnityEngine.Random.Range(0, 2);
            randomnumber = UnityEngine.Random.Range(20, 40);
            if (attack == 0)
            {
                GameObject currentBullet = Instantiate(bullet, transform);
                currentBullet.transform.localScale = new Vector3(0.5f, 5, 1);
                currentBullet.transform.position = new Vector3(transform.position.x - UnityEngine.Random.Range(0, 30) - 30, transform.position.y, 0);
                if (currentBullet.GetComponent<Collider2D>())
                    Destroy(currentBullet, 3);
            }
            else
            {
                GameObject lightning = Instantiate(Lightning, transform);
                lightning.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                lightning.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                if (lightning.GetComponent<Collider2D>())
                    Destroy(lightning, 3);
            }
        }
        
    }
}
