using UnityEngine;

public class ShopUIController : MonoBehaviour
{
    [Header("shop panels")]
    public GameObject seedPage;
    public GameObject decorPage;
    public GameObject specialPage;

    [Header("shop root")]
    public GameObject shopRoot;  


    void Start()
    {
        shopRoot.SetActive(false); 
    }

    public void OpenShop()
    {
        shopRoot.SetActive(true);
        ShowSeedPage(); 
    }

    public void CloseShop()
    {
        shopRoot.SetActive(false);
    }

    public void ShowSeedPage()
    {
        seedPage.SetActive(true);
        decorPage.SetActive(false);
        specialPage.SetActive(false);
    }

    public void ShowDecorPage()
    {
        seedPage.SetActive(false);
        decorPage.SetActive(true);
        specialPage.SetActive(false);
    }

    public void ShowSpecialPage()
    {
        seedPage.SetActive(false);
        decorPage.SetActive(false);
        specialPage.SetActive(true);
    }
}
