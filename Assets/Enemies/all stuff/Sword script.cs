using UnityEngine;

public class Swordscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private int timepassed = 0;
    private bool move = false;
    void Update()
    {
        timepassed++;
        if (move == false && timepassed * Time.deltaTime> 3)
        {
            move = true;
            timepassed = 0;
        }
        if (move&&timepassed * Time.deltaTime > 0.01f)
        {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                timepassed = 0;
        }
    }
}
