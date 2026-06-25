using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClassSelectionManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject selectionPanel;     
    public GameObject tutorialIntroPanel;  

    [Header("Text References")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI tutorialIntroText; 

    [Header("Scene Management")]
    public string tutorialSceneName = "Level0"; // Name of the tutorial scene to load - changed to Village for testing purposes, should be the tutorial scene name in final version

    private CharacterClassData pendingClass;
    private string pendingDeityName;

    //Called when player presses 'E' near a statue, passing the relevant class data and description
    public void OpenConfirmation(CharacterClassData data, string deityName, string description)
    {
        pendingClass = data;
        pendingDeityName = deityName;
        headerText.text = $"Statue of {deityName}";
        descriptionText.text = description;

        selectionPanel.SetActive(true);
    }

    // Called by "Accept Blessing" button
    public void ConfirmSelection()
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ApplyClassData(pendingClass);

            //Save the selected class to a static variable for later use in the tutorial level
            CharacterClassData.SelectedClass = pendingClass;

            SaveLoadManager.SaveGame(FindFirstObjectByType<PlayerController>(), FindFirstObjectByType<PlayerStats>(), FindFirstObjectByType<PlayerInventory>());

            //Hide selection panel and show tutorial intro
            selectionPanel.SetActive(false);

            tutorialIntroText.text = $"You have chosen the blessing of {pendingDeityName}.\nOn this first adventure you will learn your new skills.";
            tutorialIntroPanel.SetActive(true);

            AudioManager.Instance?.PlayClassChosen();
        }
    }

    //Called by the "Look further" button
    public void ClosePopup()
    {
        selectionPanel.SetActive(false);
        pendingClass = null;
    }

    //Called by "Begin Joruney" button
    public void StartTutorial()
    {
        Debug.Log("Loading Tutorial Level...");
        SceneManager.LoadScene(tutorialSceneName);
    }
}