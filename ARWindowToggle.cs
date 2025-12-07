using UnityEngine;

public class ARWindowToggle : MonoBehaviour
{
    [Header("AR Objects")]
    public GameObject arSession;     
    public GameObject xrOrigin;      
    public Camera arCamera;          

    [Header("UI")]
    public GameObject arWindowFrame; 

    [Tooltip("UI to hide")]
    public GameObject[] uiToHide;

    bool isOn = false;

    void Start()
    {        
        ApplyState();
    }

    void ApplyState()
    {
        if (arSession != null) arSession.SetActive(isOn);
        if (xrOrigin != null) xrOrigin.SetActive(isOn);
        if (arCamera != null) arCamera.enabled = isOn;
        if (arWindowFrame != null) arWindowFrame.SetActive(isOn);
       
        foreach (var ui in uiToHide)
        {
            if (ui == null) continue;
            ui.SetActive(!isOn);   
        }
    }

    public void OpenARWindow()
    {
        isOn = true;
        ApplyState();

        var tutorial = FindObjectOfType<ARTutorialManager>();
        if (tutorial != null)
        {
            tutorial.OnArOpened();
        }
    }
   
    public void CloseARWindow()
    {
        isOn = false;
        ApplyState();
    }

    public void ToggleARWindow()
    {
        isOn = !isOn;
        ApplyState();
    }
}