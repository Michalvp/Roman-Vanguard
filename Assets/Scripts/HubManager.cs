using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Statue References")]
    public GameObject minervaStatue;
    public GameObject dianaStatue;
    public GameObject herculesStatue;

    void Start()
    {
        // Hide all sratues by default
        minervaStatue.SetActive(false);
        dianaStatue.SetActive(false);
        herculesStatue.SetActive(false);

        // Check which class was selected in the menu and activate the corresponding statue
        if (CharacterClassData.SelectedClass != null)
        {
            string className = CharacterClassData.SelectedClass.className;
            Debug.Log($"Hub: Loading statue for {className}");

            //
            if (className == "Legionary") minervaStatue.SetActive(true);
            else if (className == "Archer") dianaStatue.SetActive(true);
            else if (className == "Gladiator") herculesStatue.SetActive(true);
        }
        else if(FindFirstObjectByType<PlayerController>().classData != null)
        {
            string className = FindFirstObjectByType<PlayerController>().classData.className;
            Debug.Log($"Hub: Loading statue for {className}");
            if (className == "Legionary") minervaStatue.SetActive(true);
            else if (className == "Archer") dianaStatue.SetActive(true);
            else if (className == "Gladiator") herculesStatue.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Hub: No class selected! Did you start from the Menu?");
            //Optional: You could choose to activate a default statue here, or leave them all inactive as a visual cue that something went wrong.
            // minervaStatue.SetActive(true);
        }
    }
}