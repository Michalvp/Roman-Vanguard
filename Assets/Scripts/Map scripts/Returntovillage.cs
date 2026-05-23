using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Returntovillage : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject glowObject;
    private GameObject finish;
    void Start()
    {
        finish = GameObject.FindGameObjectWithTag("Finish");
    }

    // Update is called once per frame
    public void Interact()
    {
        Mappicker.completedLevels = -1;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;
        SceneManager.LoadScene("Village");
    }
    public void SetHighlight(bool isActive)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(isActive);
        }
    }
    void Update()
    {
        
    }
}
