using UnityEngine;

public class Lightningbehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private int timepassed=0;
    void Update()
    {
        if (timepassed*Time.deltaTime >= 1)
        {
            timepassed = 0;
            Debug.Log("powinien zniknac");
            if (GetComponent<Collider2D>())
                Debug.Log("znika");
                Destroy(gameObject);
        }
        timepassed++;
    }
}
