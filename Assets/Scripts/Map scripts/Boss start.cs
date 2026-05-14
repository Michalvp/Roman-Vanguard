using UnityEngine;

public class Bossstart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject Neptun;
    [SerializeField] private GameObject Jupiter;
    [SerializeField] private GameObject Mars;
    [SerializeField] private int Bossnumber;
    void Start()
    {
        if(Bossnumber == 1)
        {
            GameObject Boss = Instantiate(Neptun, transform);
        }
        else if (Bossnumber == 2)
        {
            GameObject Boss = Instantiate(Mars, transform);
        }
        else if (Bossnumber == 3 )
        {
            GameObject Boss = Instantiate(Jupiter, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
