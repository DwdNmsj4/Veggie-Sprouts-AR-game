using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WildHarvest;

public enum FarmRoundState
{
    Idle,              
    RoundActive,        
    CoolingDown,        
    WaitingForNextColor
}

public class FarmGameManager : MonoBehaviour
{
    [Header("patch")]
    [Tooltip("all")]
    public List<PatchController> patches = new List<PatchController>();

    [Tooltip("bites per harvest")]
    [Range(1, 10)]
    public int bitesPerHarvest = 3;

    [Tooltip("starting seed color")]
    public VegType startingSeedColor = VegType.Green;

    [Header("coin settings")]
    public int currentCoins = 0;                    
    public int baseHarvestReward = 3;               
    public TextMeshProUGUI coinText;                

    [Header("next round delay")]
    public float nextRoundDelay = 5f;

    [Header("for testing")]
    [ReadOnlyInInspector] public VegType currentSeedColor;   
    [ReadOnlyInInspector] public VegType lastDetectedColor;  

    public AudioSource audioSource;   
    public AudioClip coinSFX;         

    public DeerNPC deerNPC;

    private bool tutorialStep2Shown = false;

    private FarmRoundState state = FarmRoundState.Idle;

    private bool roundEndProcessed = false;

    private bool hasStartedFirstRound = false;

    private bool firstRoundEncouragementShown = false;
    public DialogueManager dialogueManager;   

    void Start()
    {
        state = FarmRoundState.Idle;
        currentSeedColor = VegType.None;
        lastDetectedColor = VegType.None;

        UpdateCoinUI();
    }

    public void InitRound(VegType seedColor)
    {
        currentSeedColor = seedColor;
        lastDetectedColor = VegType.None;

        state = FarmRoundState.RoundActive;
        roundEndProcessed = false;

        if (patches == null || patches.Count == 0)
        {
            Debug.LogWarning("[FarmGameManager] 没有配置任何 PatchController。");
            return;
        }

        foreach (var patch in patches)
        {
            if (patch == null) continue;

            patch.farmManager = this;
            patch.InitRound(seedColor, bitesPerHarvest);
        }

        Debug.Log($"[FarmGameManager] New round started. SeedColor = {currentSeedColor}, bitesPerHarvest = {bitesPerHarvest}");
    }

    public void OnBite(VegType vegColor)
    {
        if (vegColor == VegType.None)
            return;

        if (!tutorialStep2Shown && !PlayerPrefs.HasKey("AR_Tutorial_Completed"))
        {
            var tutorial = FindObjectOfType<ARTutorialManager>();
            if (tutorial != null)
            {
                tutorial.ShowStep2();  
                tutorialStep2Shown = true;
            }
        }

        if (state == FarmRoundState.CoolingDown)
        {
            return;
        }

        if (state == FarmRoundState.Idle || state == FarmRoundState.WaitingForNextColor)
        {

            bool isFirstRound = !hasStartedFirstRound;
            InitRound(vegColor);              
            ApplyBiteToAllPatches();         
            lastDetectedColor = vegColor;     

            if
         (!isFirstRound)
            {
                var
         tutorial = FindObjectOfType<ARTutorialManager>();
                if (tutorial != null
        )
                {
                    tutorial.ShowColorHintOnce();
                   
                }
            }

            hasStartedFirstRound =
        true
        ;
            return;
        }

        if (state == FarmRoundState.RoundActive)
        {
            ApplyBiteToAllPatches();
            lastDetectedColor = vegColor;
        }
    }

  
    private void ApplyBiteToAllPatches()
    {
        if (patches == null || patches.Count == 0)
            return;

        foreach (var patch in patches)
        {
            if (patch == null) continue;
            patch.OnBite();
        }
    }

    public void OnPatchHarvested(PatchController patch)
    {
        if (patch == null)
            return;

     
        if (!firstRoundEncouragementShown)
        {
            firstRoundEncouragementShown = true;

            if (dialogueManager != null)
            {
                dialogueManager.ShowFirstRoundEncouragement();
            }
        }
      
        if (roundEndProcessed)
            return;

        roundEndProcessed = true;

        Debug.Log($"[FarmGameManager] OnPatchHarvested, seedColor = {patch.seedColor}");

        AddHarvestReward(patch.seedColor);

        state = FarmRoundState.CoolingDown;
        StartCoroutine(RoundCooldownCoroutine());
    }

 
    private IEnumerator RoundCooldownCoroutine()
    {
        if (nextRoundDelay > 0f)
        {
            yield return new WaitForSeconds(nextRoundDelay);
        }

        state = FarmRoundState.WaitingForNextColor;
    }
    private void AddHarvestReward(VegType vegColor)
    {
        int reward = baseHarvestReward;

        switch (vegColor)
        {
            case VegType.Green:
                reward = baseHarvestReward + 0;
                break;
            case VegType.Red:
                reward = baseHarvestReward + 1;
                break;
            case VegType.Orange:
                reward = baseHarvestReward + 2;
                break;
            case VegType.Purple:
                reward = baseHarvestReward + 3;
                break;
        }

        AddCoins(reward);
        Debug.Log($"[FarmGameManager] Harvest reward: +{reward} coins (vegColor = {vegColor})");
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        if (audioSource != null && coinSFX != null)
            audioSource.PlayOneShot(coinSFX);

        UpdateCoinUI();

        if (deerNPC != null)
        {
            deerNPC.PlayCelebrate();
        }
        else
        {
            Debug.LogWarning("[AddCoins] deerNPC 为空！！");
        }
    }

    public void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    public void ResetFarm()
    {
        currentCoins = 0;
        UpdateCoinUI();

        InitRound(startingSeedColor);
    }
}

public class ReadOnlyInInspectorAttribute : PropertyAttribute { }
