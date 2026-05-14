using UnityEngine;

public class Spear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public bool attack= false;
    private int attackprogress = 0;
    public bool returning = false;
    private float timepassed = 0;
    public int direction = 1;
    // Update is called once per frame
    void Update()
    {
        timepassed ++;
        if (attack && timepassed*Time.deltaTime > 0.1)
        {
            if (attackprogress < 20 && !returning)
            {
                attackprogress++;
                transform.position = new Vector3(transform.position.x + 0.1f * direction, transform.position.y, transform.position.z);

            }
            else
            {
                returning = true;
                attackprogress--;
                transform.position = new Vector3(transform.position.x - 0.1f * direction, transform.position.y, transform.position.z);
                if(attackprogress == 0)
                {
                    attack = false;
                    returning = false;
                }
            }
                timepassed = 0;
        }
    }
    
}
