using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("Top Info")]
    public TextMeshProUGUI levelAndClassText;

    [Header("Bars")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    [Header("Stats & Currency")]
    public TextMeshProUGUI skillPointsText;
    public TextMeshProUGUI denariiText;

    [Header("Skill Special Section")]
    public GameObject skillRootObject;
    public TextMeshProUGUI skillNameText;
    public Slider skillCooldownSlider;


    [Header("UI Screens")]
    public GameObject deathScreen;

    public GameObject pauseScreen;
    public GameObject controlsPopup;
    public GameObject quitToMenuPopup;
    public GameObject quitGamePopup;

    private PlayerController player;
    private float speed;
    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerController>();
        UpdateStaticInfo();
    }

    void Update()
    {
        if (PlayerStats.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeSelf)
            {
                pauseScreen.SetActive(false);
                controlsPopup.SetActive(false);
                quitToMenuPopup.SetActive(false);
                quitGamePopup.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        UpdateDynamicUI();
    }

    private void UpdateStaticInfo()
    {
        // Handle "Lvl: 0" if no class is selected yet
        string className = (CharacterClassData.SelectedClass != null) ? CharacterClassData.SelectedClass.className : "";
        int level = (PlayerStats.Instance != null) ? PlayerStats.Instance.currentLevel : 0;

        if (string.IsNullOrEmpty(className))
            levelAndClassText.text = $"Lvl: 0";
        else
            levelAndClassText.text = $"Lvl: {level} {className}";

        // Set skill name if available
        if (CharacterClassData.SelectedClass != null)
        {
            skillNameText.text = CharacterClassData.SelectedClass.className == "Archer" ? "MultiShot" :
                                 CharacterClassData.SelectedClass.className == "Legionary" ? "Vanguard Strike" : "Spartan Rage";
        }
    }

    private void UpdateDynamicUI()
    {
        // 1. Level & Class (Update in case of level up)
        string className = (CharacterClassData.SelectedClass != null) ? CharacterClassData.SelectedClass.className : "";
        levelAndClassText.text = $"Lvl: {PlayerStats.Instance.currentLevel} {className}";

        // 2. Health Bar
        healthSlider.maxValue = PlayerStats.Instance.maxHealth;
        healthSlider.value = PlayerStats.Instance.currentHealth;
        healthText.text = $"{PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth} HP";

        if (PlayerStats.Instance.currentHealth <= 0)
        {
            ShowDeathScreen();
        }

        // 3. XP Bar
        xpSlider.maxValue = PlayerStats.Instance.xpToNextLevel;
        xpSlider.value = PlayerStats.Instance.currentXP;
        xpText.text = $"{PlayerStats.Instance.currentXP} / {PlayerStats.Instance.xpToNextLevel} XP";

        // 4. Skill Points & Denarii
        skillPointsText.text = "Skill Points: " + PlayerStats.Instance.skillPoints;
        denariiText.text = "Denarii: " + PlayerStats.Instance.denarii;

        // 5. Special Skill Visibility & Cooldown
        if (PlayerStats.Instance.hasSpecialAbility)
        {
            skillRootObject.SetActive(true);
            UpdateCooldownSlider();
        }
        else
        {
            skillRootObject.SetActive(false);
        }
    }

    private void UpdateCooldownSlider()
    {
        if (player == null) return;

        float timeLeft = player.GetNextSpecialAttackTime() - Time.time;

        if (timeLeft > 0)
        {
            float maxCD = player.GetLastSpecialCooldownDuration();
            float progress = (maxCD - timeLeft) / maxCD;
            skillCooldownSlider.value = Mathf.Clamp01(progress);
        }
        else
        {
            skillCooldownSlider.value = 1f;
        }
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);

    }

    public void ReturnToHubAfterDeath()
    {
        deathScreen.SetActive(false);
        GameObject playerobj = GameObject.FindWithTag("Player");
        playerobj.transform.position = new Vector3(0, 2, 0);
        SceneManager.LoadScene("Village");
        PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
    }

    public void ClosePauseMenu()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenControlsPopup()
    {
        controlsPopup.SetActive(true);
    }

    public void CloseControlsPopup()
    {
        controlsPopup.SetActive(false);
    }

    public void OpenQuitToMenuPopup()
    {
        quitToMenuPopup.SetActive(true);
    }

    public void CloseQuitToMenuPopup()
    {
        quitToMenuPopup.SetActive(false);
    }

    public void OpenQuitGamePopup()
    {
        quitGamePopup.SetActive(true);
    }

    public void CloseQuitGamePopup()
    {
        quitGamePopup.SetActive(false);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Game has been quit.");
        Application.Quit();
    }

}