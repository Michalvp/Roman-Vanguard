using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject controlsPopup;
    public GameObject slotsPanel;
    public GameObject overwritePopup;

    private bool isNewGameMode = true;
    private int selectedSlot = -1;

    private void Start()
    {
        // Ensure all popup panels are disabled on start
        if (controlsPopup != null) controlsPopup.SetActive(false);
        if (slotsPanel != null) slotsPanel.SetActive(false);
        if (overwritePopup != null) overwritePopup.SetActive(false);
    }

    public void ShowControls()
    {
        if (controlsPopup != null)
        {
            controlsPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ControlsPopup is not assigned in MainMenuManager!");
        }
    }

    public void HideControls()
    {
        if (controlsPopup != null)
        {
            controlsPopup.SetActive(false);
        }
    }
    public void QuitGame()
    {
        Debug.Log("Game has been quit!");

        Application.Quit();
    }

    //Opens save slots when New Game is clicked. Sets the mode to New Game so that the correct actions are taken when a slot is selected.
    public void OpenNewGameSlots()
    {
        isNewGameMode = true;
        if (slotsPanel != null) slotsPanel.SetActive(true);
    }

    //Opens save slots when Load Game is clicked. Sets the mode to Load Game so that the correct actions are taken when a slot is selected.
    public void OpenLoadGameSlots()
    {
        isNewGameMode = false;
        if (slotsPanel != null) slotsPanel.SetActive(true);
    }

    public void CloseSlotsPanel()
    {
        if (slotsPanel != null) slotsPanel.SetActive(false);
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;
        bool isSlotEmpty = CheckIfSlotIsEmpty(slotIndex);

        if (isNewGameMode)
        {
            // If it's a new game and slot is empty, start immediately
            if (isSlotEmpty)
            {
                StartNewGame(slotIndex);
            }
            // If slot is taken, show overwrite confirmation
            else
            {
                if (overwritePopup != null) overwritePopup.SetActive(true);
            }
        }
        else
        {
            // If it's load game mode and slot has data, load it
            if (!isSlotEmpty)
            {
                LoadExistingGame(slotIndex);
            }
            else
            {
                // Optionally show a warning UI here that the slot is empty
                Debug.LogWarning("Selected slot is empty. Cannot load.");
            }
        }
    }

    public void ConfirmOverwrite()
    {
        if (overwritePopup != null) overwritePopup.SetActive(false);
        StartNewGame(selectedSlot);
    }

    public void CancelOverwrite()
    {
        if (overwritePopup != null) overwritePopup.SetActive(false);
    }

    private void StartNewGame(int slotIndex)
    {
        Debug.Log("Starting new game on slot: " + slotIndex);

        // Save the active slot globally so other scripts know which slot to use
        PlayerPrefs.SetInt("CurrentSaveSlot", slotIndex);

        // Mark this slot as "not empty" for future checks
        PlayerPrefs.SetInt("SlotHasData_" + slotIndex, 1);
        PlayerPrefs.Save();

        // Load class selection scene
        SceneManager.LoadScene("ClassSelection");
    }

    private void LoadExistingGame(int slotIndex)
    {
        Debug.Log("Loading game from slot: " + slotIndex);

        // Save the active slot globally
        PlayerPrefs.SetInt("CurrentSaveSlot", slotIndex);

        // Load village scene
        SceneManager.LoadScene("Village");
    }

    //Temporary method to check if a slot is empty. In a real implementation, this would likely check for the existence of a save file or specific save data.
    private bool CheckIfSlotIsEmpty(int slotIndex)
    {
        // Returns true if the key doesn't exist or is set to 0
        return PlayerPrefs.GetInt("SlotHasData_" + slotIndex, 0) == 0;
    }
}