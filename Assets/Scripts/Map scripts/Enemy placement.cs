using Unity.VisualScripting;
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
    private int randomnum;
    void Start()
    {
        Finish= GameObject.FindGameObjectWithTag("Finish");
        randomnum = Random.Range(0, 100);
        int finishedmaps = Mappicker.completedLevels;
                Debug.Log(finishedmaps);
        if (randomnum < 34)
        {
            randomnum= Random.Range(0, 100);
            if (randomnum < finishedmaps * 25)
            {
                GameObject currentEliteSoldier = Instantiate(EliteSoldier, transform);
                currentEliteSoldier.GetComponentInChildren<Elitesoldierscript>().setstats(100+ 50*finishedmaps,10+5*finishedmaps,finishedmaps*2);
            }
            else
            {
                GameObject currentMeleeEnemy = Instantiate(MeleeEnemy, transform);
                currentMeleeEnemy.GetComponentInChildren<Meleeenemyscript>().setstats(100 + 50 * finishedmaps, 5+5 * finishedmaps, finishedmaps);
            }
        }
        else if( randomnum < 67)
        {
            randomnum = Random.Range(0, 100);
            if (randomnum < finishedmaps * 25)
            {
                GameObject currentEliteShieldedenemy = Instantiate(EliteShieldedenemy, transform);
                currentEliteShieldedenemy.GetComponentInChildren<EliteMeleeenemywithshield>().setstats(50 + 50 * finishedmaps,10*  10 * finishedmaps,5+ finishedmaps*5);
            }
            else
            {
                GameObject currentShieldedenemy = Instantiate(Shieldedenemy, transform);
                currentShieldedenemy.GetComponentInChildren<Meleeenemywithshield>().setstats(50+50 * finishedmaps,5+ 5 * finishedmaps,2+ finishedmaps * 3);
            }
        }
        else
        {
            randomnum = Random.Range(0, 100);
            if (randomnum < finishedmaps * 25)
            {
                GameObject currentRangedEnemy = Instantiate(EliteArcher, transform);
                currentRangedEnemy.GetComponentInChildren<EliteArcher>().setstats(50+ 50 * finishedmaps,20+ 20 * finishedmaps, 0);
            }
            else
            {
                GameObject currentRangedEnemy = Instantiate(RangedEnemy, transform);
                currentRangedEnemy.GetComponentInChildren<Rangedenemy>().setstats(25+25 * finishedmaps,10+ 10 * finishedmaps, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        
    }
}
