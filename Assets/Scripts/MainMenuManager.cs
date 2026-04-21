using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class MainMenuManager : MonoBehaviour
{
    // Function for the New Game button
    public void NewGame()
    {
        Debug.Log("Starting New Game... Loading Class Selection.");
        // Make sure the name matches your Class Selection scene exactly!
        SceneManager.LoadScene("ClassSelection");
    }

    // Function for the Load Game button
    public void LoadGame()
    {
        // For now, it just logs a message
        Debug.Log("Load Game pressed. Feature not implemented yet.");
    }
}