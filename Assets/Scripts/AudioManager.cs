using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Xml.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;
public enum EnemyAttackSoundGroup
{
    Melee,
    Ranged,
    Boss
}
public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Source: Background Music")]
    [SerializeField] private AudioSource musicSource;
    [Header("Source: Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [Header("Volume: Background Music")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.55f;
    [Header("Volume: All Sound Effects")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.85f;
    [Header("Volume: Player Attack Sounds")]
    [Range(0f, 1f)]
    [SerializeField] private float playerAttackVolume = 0.85f;
    [Header("Volume: Enemy Attack Sounds")]
    [Range(0f, 1f)]
    [SerializeField] private float enemyAttackVolume = 0.85f;
    [Header("Volume: Movement Sounds")]
    [Range(0f, 1f)]
    [SerializeField] private float movementVolume = 0.65f;
    [Header("Volume: UI, Shop, Coin, and Level Sounds")]
    [Range(0f, 1f)]
    [SerializeField] private float uiAndPickupVolume = 0.80f;
    [Header("Volume: Player Hurt and Death Sounds")]
    [Range(0f, 1f)]
    [SerializeField] private float damageVolume = 0.90f;
    [Header("Timing: Music Fade Duration in Seconds")]
    [Min(0f)]
    [SerializeField] private float musicFadeSeconds = 0.45f;
    [Header("Timing: Minimum Seconds Between Footsteps")]
    [Min(0.05f)]
    [SerializeField] private float footstepCooldown = 0.32f;
    [Header("Music Clip: Main Menu")]
    [SerializeField] private AudioClip mainMenuMusic;
    [Header("Music Clip: Class Selection")]
    [SerializeField] private AudioClip classSelectionMusic;
    [Header("Music Clip: Calm Village")]
    [SerializeField] private AudioClip villageMusic;
    [Header("Music Clip: Combat Levels")]
    [SerializeField] private AudioClip combatMusic;
    [Header("Music Clip: Boss Levels")]
    [SerializeField] private AudioClip bossMusic;
    [Header("SFX Clips: Legionary Attack")]
    [SerializeField] private AudioClip[] legionaryAttackClips;
    [Header("SFX Clips: Gladiator Attack")]
    [SerializeField] private AudioClip[] gladiatorAttackClips;
    [Header("SFX Clips: Archer Attack")]
    [SerializeField] private AudioClip[] archerAttackClips;
    [Header("SFX Clips: Melee Enemy Attack")]
    [SerializeField] private AudioClip[] meleeEnemyAttackClips;
    [Header("SFX Clips: Ranged Enemy Attack")]
    [SerializeField] private AudioClip[] rangedEnemyAttackClips;
    [Header("SFX Clips: Boss Attack")]
    [SerializeField] private AudioClip[] bossAttackClips;
    [Header("SFX Clip: Enter Shop")]
    [SerializeField] private AudioClip enterShopClip;
    [Header("SFX Clip: Pick Up Coin")]
    [SerializeField] private AudioClip coinPickupClip;
    [Header("SFX Clip: Choose Player Class")]
    [SerializeField] private AudioClip classChosenClip;
    [Header("SFX Clip: Enter Combat Level")]
    [SerializeField] private AudioClip enterLevelClip;
    [Header("SFX Clips: Walking Footsteps")]
    [SerializeField] private AudioClip[] footstepClips;
    [Header("SFX Clip: Jump")]
    [SerializeField] private AudioClip jumpClip;
    [Header("SFX Clip: Sell Item")]
    [SerializeField] private AudioClip itemSoldClip;
    [Header("SFX Clip: Player Receives Damage")]
    [SerializeField] private AudioClip playerHurtClip;
    [Header("SFX Clip: Player Dies")]
    [SerializeField] private AudioClip playerDeathClip;
    [Header("Scene Name: Main Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Header("Scene Name: Class Selection")]
    [SerializeField] private string classSelectionSceneName = "ClassSelection";
    [Header("Scene Name: Village")]
    [SerializeField] private string villageSceneName = "Village";
    [Header("Scene Names: Combat Levels")]
    [SerializeField] private string[] combatSceneNames = { "Level0" };
    [Header("Scene Names: Boss Levels")]
    [SerializeField] private string[] bossSceneNames;
    private Coroutine musicFadeRoutine;
    private float nextFootstepTime;
    private bool sceneLoadedCallbackReceived;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError(
            "AudioManager needs both Music Source and SFX Source assigned in the Inspector.",
            this);
            return;
        }
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = sfxVolume;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }
    private void Start()
    {
        // Fallback for unusual cases where this manager did not receive
        // the sceneLoaded callback before Start.
        if (!sceneLoadedCallbackReceived)
        {
            ApplyAudioForScene(SceneManager.GetActiveScene(), false);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Instance = null;
        }
    }
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoadedCallbackReceived = true;
        ApplyAudioForScene(scene, true);
    }
    private void ApplyAudioForScene(Scene scene, bool allowLevelEntrySound)
    {
        bool isBossScene = SceneListContains(bossSceneNames, scene.name);
        bool isCombatScene = SceneListContains(combatSceneNames, scene.name);
        if (scene.name == mainMenuSceneName)
        {
            PlayMusic(mainMenuMusic);
        }
        else if (scene.name == classSelectionSceneName)
        {
            PlayMusic(classSelectionMusic);
        }
        else if (scene.name == villageSceneName)
        {
            PlayMusic(villageMusic);
        }
        else if (isBossScene)
        {
            PlayMusic(bossMusic != null ? bossMusic : combatMusic);
        }
        else if (isCombatScene)
        {
            PlayMusic(combatMusic);
        }
        if (allowLevelEntrySound && (isCombatScene || isBossScene))
        {
            PlayEnterLevel();
        }
    }
    private static bool SceneListContains(string[] sceneNames, string currentSceneName)
    {
        if (sceneNames == null)
        {
            return false;
        }
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == currentSceneName)
            {
                return true;
            }
        }
        return false;
    }
    public void PlayPlayerAttack(string className)
    {
        switch (className)
        {
            case "Legionary":
                PlayRandomClip(legionaryAttackClips, playerAttackVolume);
                break;
            case "Gladiator":
                PlayRandomClip(gladiatorAttackClips, playerAttackVolume);
                break;
            case "Archer":
                PlayRandomClip(archerAttackClips, playerAttackVolume);
                break;
            default:
                Debug.LogWarning("No player attack audio group matches class: " + className, this);
                break;
        }
    }
    public void PlayEnemyAttack(EnemyAttackSoundGroup group)
    {
        switch (group)
        {
            case EnemyAttackSoundGroup.Melee:
                PlayRandomClip(meleeEnemyAttackClips, enemyAttackVolume);
                break;
            case EnemyAttackSoundGroup.Ranged:
                PlayRandomClip(rangedEnemyAttackClips, enemyAttackVolume);
                break;
            case EnemyAttackSoundGroup.Boss:
                PlayRandomClip(bossAttackClips, enemyAttackVolume);
                break;
        }
    }
    public void PlayEnterShop()
    {
        PlaySingleClip(enterShopClip, uiAndPickupVolume);
    }
    public void PlayCoinPickup()
    {
        PlaySingleClip(coinPickupClip, uiAndPickupVolume);
    }
    public void PlayClassChosen()
    {
        PlaySingleClip(classChosenClip, uiAndPickupVolume);
    }
    public void PlayEnterLevel()
    {
        PlaySingleClip(enterLevelClip, uiAndPickupVolume);
    }
    public void PlayFootstep()
    {
        if (Time.time < nextFootstepTime)
        {
            return;
        }
        nextFootstepTime = Time.time + footstepCooldown;
        PlayRandomClip(footstepClips, movementVolume);
    }
    public void PlayJump()
    {
        PlaySingleClip(jumpClip, movementVolume);
    }
    public void PlayItemSold()
    {
        PlaySingleClip(itemSoldClip, uiAndPickupVolume);
    }
    public void PlayPlayerHurt()
    {
        PlaySingleClip(playerHurtClip, damageVolume);
    }
    public void PlayPlayerDeath()
    {
        PlaySingleClip(playerDeathClip, damageVolume);
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    private void PlaySingleClip(AudioClip clip, float volumeScale)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }
    private void PlayRandomClip(AudioClip[] clips, float volumeScale)
    {
        if (sfxSource == null || clips == null || clips.Length == 0)
        {
            return;
        }
        // Start at a random position, then inspect every slot once.
        // This keeps the choice random while still finding a valid clip
        // when one of the Inspector array elements was left empty.
        int randomStart = UnityEngine.Random.Range(0, clips.Length);
        for (int offset = 0; offset < clips.Length; offset++)
        {
            int index = (randomStart + offset) % clips.Length;
            AudioClip clip = clips[index];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
                return;
            }
        }
    }
    private void PlayMusic(AudioClip nextClip)
    {
        if (musicSource == null || nextClip == null)
        {
            return;
        }
        if (musicSource.clip == nextClip && musicSource.isPlaying)
        {
            musicSource.volume = musicVolume;
            return;
        }
        if (musicFadeRoutine != null)
        {
            StopCoroutine(musicFadeRoutine);
        }
        musicFadeRoutine = StartCoroutine(FadeToMusic(nextClip));
    }
    private IEnumerator FadeToMusic(AudioClip nextClip)
    {
        float fadeDuration = Mathf.Max(0.01f, musicFadeSeconds);
        if (musicSource.isPlaying)
        {
            float startingVolume = musicSource.volume;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startingVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }
        musicSource.clip = nextClip;
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.Play();
        float fadeInElapsed = 0f;
        while (fadeInElapsed < fadeDuration)
        {
            fadeInElapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, fadeInElapsed / fadeDuration);
            yield return null;
        }
        musicSource.volume = musicVolume;
        musicFadeRoutine = null;
    }
}