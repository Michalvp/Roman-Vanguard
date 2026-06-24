using UnityEngine;
using UnityEngine.SceneManagement;

public class Mappicker : MonoBehaviour,IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static int completedLevels =-1;
    private int randomnum;
    public GameObject glowObject;

    public GameObject continuePopup;
    void Start()
    {
        Debug.Log(completedLevels);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetHighlight(bool isActive)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(isActive);
        }
    }
    public void Interact()
    {
        if (continuePopup != null)
        {
            continuePopup.SetActive(true);
        }
        else
        {
            Debug.LogError("Continue popup is not assigned in the inspector.");
        }
    }
    private void LevelCompleted()
    {
        completedLevels++;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;
        SaveLoadManager.SaveGame(FindFirstObjectByType<PlayerController>(), FindFirstObjectByType<PlayerStats>(), FindFirstObjectByType<PlayerInventory>());
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

    public void CancelContinue()
    {
        if (continuePopup != null)
        {
            continuePopup.SetActive(false);
        }
    }

    public void ConfirmContinue()
    {
        LevelCompleted();
    }
}
