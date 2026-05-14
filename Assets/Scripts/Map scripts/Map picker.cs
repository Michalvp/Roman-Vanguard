using UnityEngine;
using UnityEngine.SceneManagement;

public class Mappicker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int completedLevels =-1;
    private int randomnum;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelCompleted();
        }
    }
    private void LevelCompleted()
    {
        completedLevels++;
        randomnum = Random.Range(0, 100);
        if (randomnum < 100 - completedLevels * 25)
        {
            do
            {
            randomnum = Random.Range(1, 6);
            }while (randomnum == SceneManager.GetActiveScene().buildIndex+1);

            SceneManager.LoadScene("room " + randomnum);
        }
        else if (randomnum < 100 - (completedLevels - 1) * 10)
        {
            do
            {
            randomnum = Random.Range(6, 11);
                
            } while (randomnum == SceneManager.GetActiveScene().buildIndex+1);
            SceneManager.LoadScene("room " + randomnum);
        }
        else
        {
            do
            {
            randomnum = Random.Range(11, 16);
            } while (randomnum == SceneManager.GetActiveScene().buildIndex+1);
            SceneManager.LoadScene("room " + randomnum);
        }
    }
}
