using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Enemyplacement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject RangedEnemy;
    [SerializeField] private GameObject MeleeEnemy;
    [SerializeField] private GameObject EliteSoldier;
    [SerializeField] private GameObject EliteArcher;
    [SerializeField] private GameObject Shieldedenemy;
    [SerializeField] private GameObject EliteShieldedenemy;

    private GameObject Finish;
    [SerializeField] private Transform pos;
    private int randomnum;
    void Start()
    {
        Finish= GameObject.FindGameObjectWithTag("Finish");
        randomnum = Random.Range(0, 100);
        if (randomnum < 34)
        {
            randomnum= Random.Range(0, 100);
            if (randomnum < Finish.GetComponent<Mappicker>().completedLevels * 25)
            {
                GameObject currentEliteSoldier = Instantiate(EliteSoldier, transform);
            }
            else
            {
                GameObject currentMeleeEnemy = Instantiate(MeleeEnemy, transform);

            }
        }
        else if( randomnum < 67)
        {
            randomnum = Random.Range(0, 100);
            if (randomnum < Finish.GetComponent<Mappicker>().completedLevels * 25)
            {
                GameObject currentEliteShieldedenemy = Instantiate(EliteShieldedenemy, transform);
            }
            else
            {
                GameObject currentShieldedenemy = Instantiate(Shieldedenemy, transform);
            }
        }
        else
        {
            randomnum = Random.Range(0, 100);
            if (randomnum < Finish.GetComponent<Mappicker>().completedLevels * 25)
            {

                GameObject currentRangedEnemy = Instantiate(EliteArcher, transform);
            }
            else
            {
                GameObject currentRangedEnemy = Instantiate(RangedEnemy, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        
    }
}
