using UnityEngine;

public class ARTutorialManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    public GameObject step1Panel;
    public GameObject step2Panel;
    public GameObject step3Panel;
    public GameObject colorHintPanel;


    [Header("AR system")]
    public GameObject arSystem;   
    void Start()
    {
        HideAll();
    }
    public void OnArOpened()
    {
        if (PlayerPrefs.HasKey("AR_Tutorial_Completed"))
        {
            if (arSystem != null) arSystem.SetActive(true);
            return;
        }

        ShowStep1();
        if (arSystem != null) arSystem.SetActive(true);
    }

    public void ShowStep1()
    {
        step1Panel.SetActive(true);
        step2Panel.SetActive(false);
        step3Panel.SetActive(false);
    }

    public void ShowStep2()
    {
        step1Panel.SetActive(false);
        step2Panel.SetActive(true);
        step3Panel.SetActive(false);
        Invoke(nameof(ShowStep3), 1.5f);
    }

    private void AutoFinishStep3()
    {
        FinishTutorial();
    }
    public void ShowStep3()
    {
        step1Panel.SetActive(false);
        step2Panel.SetActive(false);
        step3Panel.SetActive(true);
        Invoke(nameof(AutoFinishStep3), 5f);
    }

    public void FinishTutorial()
    {
        PlayerPrefs.SetInt("AR_Tutorial_Completed", 1);

        HideAll();

        if (arSystem != null)
            arSystem.SetActive(true);
    }

    private bool colorHintShown = false;

    public void ShowColorHintOnce()
    {
        if (colorHintShown) return;  
        colorHintShown = true;

        if (colorHintPanel != null)
        {
            colorHintPanel.SetActive(true);
            CancelInvoke(nameof(HideColorHint));
            Invoke(nameof(HideColorHint), 2.5f); 
        }
    }

    void HideColorHint()
    {
        if (colorHintPanel != null)
            colorHintPanel.SetActive(false);
    }
    private void HideAll()
    {
        if (step1Panel != null) step1Panel.SetActive(false);
        if (step2Panel != null) step2Panel.SetActive(false);
        if (step3Panel != null) step3Panel.SetActive(false);
    }
}
