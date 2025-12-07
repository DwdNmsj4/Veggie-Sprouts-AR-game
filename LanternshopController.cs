using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanternShopController : MonoBehaviour
{
    [Header("coin system")]
    public FarmGameManager gameManager;   

    [Header("lantern")]
    public Button lanternButton;          
    public int lanternPrice = 8;          
    public GameObject lanternOnBuilding;  

    [Header("confirm panel")]
    public GameObject confirmPanel;       
    public Button confirmYesButton;
    public Button confirmNoButton;

    [Header("other item buttons")]
    public Button[] otherItemButtons;     

    bool lanternPurchased = false;       



    void Start()
    {
        
        lanternButton.onClick.AddListener(OnLanternButtonClicked);
        confirmYesButton.onClick.AddListener(OnConfirmYes);
        confirmNoButton.onClick.AddListener(OnConfirmNo);

        
        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        
        UpdateShopUI();
    }


    public void OnShopOpened()
    {
        UpdateShopUI();
    }

    void UpdateShopUI()
    {
       
        bool canBuyLantern =
            !lanternPurchased &&                
            gameManager.currentCoins >= lanternPrice;

        lanternButton.interactable = canBuyLantern;

        if (otherItemButtons != null)
        {
            foreach (var btn in otherItemButtons)
            {
                if (btn != null)
                    btn.interactable = false;
            }
        }
    }

    public void OnLanternButtonClicked()
    {
       
        if (lanternPurchased)
            return;

        if (gameManager.currentCoins < lanternPrice)
            return;

        if (confirmPanel != null)
            confirmPanel.SetActive(true);
    }

    public void OnConfirmYes()
    {
        lanternPurchased = true;

        if (lanternOnBuilding != null)
            lanternOnBuilding.SetActive(true);

        if (gameManager != null)
        {
            gameManager.currentCoins = 0;

            gameManager.UpdateCoinUI();
        }

       
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }


    void OnConfirmNo()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }
}
