using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro; // Required for TextMeshPro components

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject controlsPopup;
    public GameObject creatrosPopup;
    public GameObject slotsPanel;
    public GameObject overwritePopup;

    [Header("Slot Settings")]
    [Tooltip("Assign the 5 TextMeshProUGUI text components located next to the slot buttons.")]
    public TextMeshProUGUI[] slotTexts;

    private bool isNewGameMode = true;
    private int selectedSlot = -1;

    private void Start()
    {
        if (controlsPopup != null) controlsPopup.SetActive(false);
        if (creatrosPopup != null) creatrosPopup.SetActive(false);
        if (slotsPanel != null) slotsPanel.SetActive(false);
        if (overwritePopup != null) overwritePopup.SetActive(false);
    }

    // --- BASIC BUTTON METHODS --- //

    public void ShowControls()
    {
        if (controlsPopup != null) controlsPopup.SetActive(true);
    }

    public void HideControls()
    {
        if (controlsPopup != null) controlsPopup.SetActive(false);
    }

    public void ShowCreators()
    {
        if (creatrosPopup != null) creatrosPopup.SetActive(true);
    }

    public void HideCreators()
    {
        if (creatrosPopup != null) creatrosPopup.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game has been quit.");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }

    // --- SLOT PANEL LOGIC --- //

    public void OpenNewGameSlots()
    {
        isNewGameMode = true;
        UpdateSlotTexts();
        if (slotsPanel != null) slotsPanel.SetActive(true);
    }

    public void OpenLoadGameSlots()
    {
        isNewGameMode = false;
        UpdateSlotTexts();
        if (slotsPanel != null) slotsPanel.SetActive(true);
    }

    public void CloseSlotsPanel()
    {
        if (slotsPanel != null) slotsPanel.SetActive(false);
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;
        bool isSlotEmpty = !HasSaveData(slotIndex);

        if (isNewGameMode)
        {
            if (isSlotEmpty)
            {
                StartNewGame(slotIndex);
            }
            else
            {
                if (overwritePopup != null) overwritePopup.SetActive(true);
            }
        }
        else
        {
            if (!isSlotEmpty)
            {
                LoadExistingGame(slotIndex);
            }
            else
            {
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

    // --- SCENE LOADING & SAVE INITIALIZATION --- //

    private void StartNewGame(int slotIndex)
    {
        Debug.Log("Starting new game on slot: " + slotIndex);

        // Save the active slot globally so SaveLoadManager can use it later
        PlayerPrefs.SetInt("CurrentSaveSlot", slotIndex);
        PlayerPrefs.Save();

        // Initialize an empty/dummy save file to mark the slot as used
        CreateInitialSaveFile(slotIndex);

        SceneManager.LoadScene("ClassSelection");
    }

    private void LoadExistingGame(int slotIndex)
    {
        Debug.Log("Loading game from slot: " + slotIndex);
        PlayerPrefs.SetInt("CurrentSaveSlot", slotIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Village");
    }

    // --- FILE SYSTEM (JSON) UTILITIES --- //

    private string GetSaveFilePath(int slotIndex)
    {
        // Creates a path like: C:/Users/User/AppData/LocalLow/YourStudio/RomanVanguard/save_slot_1.json
        return Path.Combine(Application.persistentDataPath, "save_slot_" + slotIndex + ".json");
    }

    private bool HasSaveData(int slotIndex)
    {
        return File.Exists(GetSaveFilePath(slotIndex));
    }

    // Reads the JSON files and updates the 5 TextMeshPro fields in the UI.
    private void UpdateSlotTexts()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            int slotNumber = i + 1;

            if (HasSaveData(slotNumber))
            {
                string path = GetSaveFilePath(slotNumber);
                string jsonContent = File.ReadAllText(path);

                // Deserializes JSON into a temporary object to grab the metadata
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(jsonContent);

                // Formats the text string appropriately
                slotTexts[i].text = $"In use - Class: {data.characterClassName}, Last played: {data.lastPlayedDate}";
            }
            else
            {
                slotTexts[i].text = "Not in use";
            }
        }
    }

    private void CreateInitialSaveFile(int slotIndex)
    {
        GameSaveData initialData = new GameSaveData();
        initialData.lastPlayedDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        initialData.characterClassName = "Selecting...";

        string json = JsonUtility.ToJson(initialData, true); // true param formats JSON with indents
        File.WriteAllText(GetSaveFilePath(slotIndex), json);
    }
}
