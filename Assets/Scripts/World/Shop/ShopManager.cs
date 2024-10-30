using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    string itemName;
    int currency;
    Button btn;

    public Button flashlight, gun;


    private void Start()
    {
        if (PlayerPrefs.HasKey("Gun") && PlayerPrefs.GetInt("Gun") != 0) gun.enabled = false;
        if (PlayerPrefs.HasKey("Flashlight") && PlayerPrefs.GetInt("Flashlight") != 0) flashlight.enabled = false;
    }

    private void UpgradeItem()
    {
        if (Inventory.Instance.UpgradeItem(itemName, currency))
        {
            btn.enabled = false;
            if (itemName == "Flashlight") PlayerPrefs.SetInt("Flashlight", 1);
            if (itemName == "Gun") PlayerPrefs.SetInt("Gun", 1);
        }
    }

    public void UpgradeItem(string name)
    {
        this.itemName = name;
    }

    public void UpgradeItem(int currency)
    {
        this.currency = currency;
    }

    public void UpgradeItem(Button btn)
    {
        this.btn = btn;

        UpgradeItem();
    }

    public void ExitShop()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.UnloadSceneAsync("Shop");
    }
}
