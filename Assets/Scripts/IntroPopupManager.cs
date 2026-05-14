using UnityEngine;

public class IntroPopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public MonoBehaviour playerScript; 

    void Start()
    {
        //Show the popup and "pause" the player
        popupPanel.SetActive(true);
        if (playerScript != null) playerScript.enabled = false;

        //If needed, add additional logic to pause time or disable other player controls here
    }

    public void ClosePopup()
    {
        //Hide the popup and "unpause" the player
        popupPanel.SetActive(false);
        if (playerScript != null) playerScript.enabled = true;

        //If needed, resume time or re-enable other player controls here

        Debug.Log("The player is ready to meet the gods.");
    }
}