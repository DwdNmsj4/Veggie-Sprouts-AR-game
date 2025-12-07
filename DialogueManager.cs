using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("dialogue UI")]
    public GameObject dialoguePanel;          
    public TextMeshProUGUI dialogueText;      

    public Button nextButton;                 

    public GameObject choicesGroup;           
    public Button choiceButtonA;              
    public Button choiceButtonB;              
    public TextMeshProUGUI choiceAText;       
    public TextMeshProUGUI choiceBText;       

    [Header("task UI")]
    public GameObject taskPanel;              
    public GameObject task2Panel;
    private bool firstTaskFinished = false;
    private bool encouragementShown = false;

    void Start()
    {
        SetupIntro();
    }
    void SetupIntro()
    {
        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (taskPanel) taskPanel.SetActive(false);

        if (choicesGroup) choicesGroup.SetActive(false); 
        if (dialogueText)
            dialogueText.text = "Hi! let's take care of your farm today!";

        if (nextButton)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnClickNextFromIntro);
        }
    }

    void OnClickNextFromIntro()
    {
        if (dialogueText)
            dialogueText.text = "How do you wanna start your task today?";

        if (nextButton)
            nextButton.gameObject.SetActive(false);

        if (choicesGroup)
            choicesGroup.SetActive(true);

        if (choiceAText) choiceAText.text = "Tell me.";
        if (choiceBText) choiceBText.text = "I'll try!";

        if (choiceButtonA) choiceButtonA.onClick.RemoveAllListeners();
        if (choiceButtonB) choiceButtonB.onClick.RemoveAllListeners();

        if (choiceButtonA) choiceButtonA.onClick.AddListener(OnChooseHelp);
        if (choiceButtonB) choiceButtonB.onClick.AddListener(OnChooseSkip);
    }

    void OnChooseHelp()
    {
        if (dialogueText)
            dialogueText.text = "Well, let's see your task today.";

        if (choicesGroup)
            choicesGroup.SetActive(false);

        ShowTaskPanel();

    }

    void OnChooseSkip()
    {
        if (dialogueText)
            dialogueText.text = "Well, let's see your task today!";

        if (choicesGroup)
            choicesGroup.SetActive(false);

        ShowTaskPanel();

    }

    public void ShowTaskPanel()
    {
        if (taskPanel)
            taskPanel.SetActive(true);

        if (dialoguePanel)
            dialoguePanel.SetActive(true);

        if (nextButton) nextButton.gameObject.SetActive(false);
        if (choicesGroup) choicesGroup.SetActive(false);

        if (firstTaskFinished)
        {
            if (taskPanel) taskPanel.SetActive(true);
            if (task2Panel) task2Panel.SetActive(false);
        }
        else
        {
            if (taskPanel) taskPanel.SetActive(false);
            if (task2Panel) task2Panel.SetActive(true);
        }
    }

    public void OnTaskReadyButtonClicked()
    {
        if (taskPanel)
            taskPanel.SetActive(false);
     
        if (dialoguePanel) dialoguePanel.SetActive(true);

        if (dialogueText)
            dialogueText.text = "Let's get started!";

        StartCoroutine(HideDialogueAfterDelay(5f));

        firstTaskFinished = true;
    }

    public void ShowFirstRoundEncouragement()
    {
        if (encouragementShown) return;       
        encouragementShown = true;            

        if (dialoguePanel) dialoguePanel.SetActive(true);

        if (nextButton) nextButton.gameObject.SetActive(false);
        if (choicesGroup) choicesGroup.SetActive(false);

        if (dialogueText)
            dialogueText.text =
                "Great job! Your first harvest is done!\n" +
                "<size=80%>Check task 2!</size>";

        StopAllCoroutines();
        StartCoroutine(HideDialogueAfterDelay(7f));
    }
    IEnumerator HideDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialoguePanel) dialoguePanel.SetActive(false);
    }
}
