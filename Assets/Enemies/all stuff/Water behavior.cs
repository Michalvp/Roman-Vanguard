using UnityEngine;

public class Waterbehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private int timepassed=0;
    private int waterrise = 0;
    private int waterdrytime = 0;
    // Update is called once per frame
    void Update()
    {
        timepassed++;
        if (timepassed * Time.deltaTime > 0.5f)
       {    if (waterrise < 30)
           {
               waterrise++;
            transform.position = new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z);
            }
            else
            {
                waterdrytime++;
                if(waterdrytime > 20)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                }
                if(waterdrytime > 40)
                    Destroy(gameObject);
            }
                timepassed = 0;
       }
    }
}
