using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public GameObject shopPanel;
    public GameObject openShopButton;
    public GameObject cartPanel;
    public TMP_Text moneyText;

    public List<Upgrade> allUpgrades = new List<Upgrade>();
    public GameObject upgradePrefab;
    public Transform upgradesPanel;

    public int playerMoney = 1000;
    private List<UpgradeItem> cartItems = new List<UpgradeItem>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateMoneyDisplay();
        openShopButton.GetComponent<Button>().onClick.AddListener(ToggleShop);
    }

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    public void ShowUpgrades(string category)
    {
        // Clear existing upgrades
        foreach (Transform child in upgradesPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate upgrades for the selected category
        foreach (Upgrade upgrade in allUpgrades)
        {
            if (upgrade.category == category)
            {
                GameObject upgradeGO = Instantiate(upgradePrefab, upgradesPanel);
                UpgradeItem upgradeItem = upgradeGO.GetComponent<UpgradeItem>();
                upgradeItem.upgrade = upgrade;
                upgradeGO.GetComponentInChildren<Text>().text = upgrade.name + "\nCost: " + upgrade.cost;
            }
        }
    }

    public void AddToCart(UpgradeItem item)
    {
        cartItems.Add(item);
    }

    public void ClearCart()
    {
        foreach (UpgradeItem item in cartItems)
        {
            Destroy(item.gameObject);
        }
        cartItems.Clear();
    }

    public void ConfirmPurchase()
    {
        int totalCost = 0;
        foreach (UpgradeItem item in cartItems)
        {
            totalCost += item.upgrade.cost;
        }

        if (playerMoney >= totalCost)
        {
            playerMoney -= totalCost;
            UpdateMoneyDisplay();
            ClearCart();
            Debug.Log("Purchase successful!");
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void UpdateMoneyDisplay()
    {
        moneyText.text = "Money: " + playerMoney;
    }
}