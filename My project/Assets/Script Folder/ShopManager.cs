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

    // Track quantities in the cart
    private Dictionary<Upgrade, int> cartQuantities = new Dictionary<Upgrade, int>();
    // Track how many of each upgrade the player has already purchased
    private Dictionary<Upgrade, int> purchasedUpgrades = new Dictionary<Upgrade, int>();
    // Track the GameObjects in the cart for each upgrade
    private Dictionary<Upgrade, GameObject> cartItemObjects = new Dictionary<Upgrade, GameObject>();

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
        foreach (Transform child in upgradesPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Upgrade upgrade in allUpgrades)
        {
            if (upgrade.category == category)
            {
                GameObject upgradeGO = Instantiate(upgradePrefab, upgradesPanel);
                UpgradeItem upgradeItem = upgradeGO.GetComponent<UpgradeItem>();
                if (upgradeItem != null)
                {
                    upgradeItem.upgrade = upgrade;
                }

                TMPro.TMP_Text[] texts = upgradeGO.GetComponentsInChildren<TMPro.TMP_Text>();
                if (texts.Length > 0) texts[0].text = upgrade.name;
                if (texts.Length > 1) texts[1].text = "Cost: " + upgrade.cost;
            }
        }
    }

    // Add an upgrade to the cart (duplicates allowed)
    public void AddToCart(Upgrade upgrade)
    {
        // Check if adding this upgrade would exceed the limit
        int currentCartQty = cartQuantities.ContainsKey(upgrade) ? cartQuantities[upgrade] : 0;
        int currentPurchasedQty = purchasedUpgrades.ContainsKey(upgrade) ? purchasedUpgrades[upgrade] : 0;

        if (currentCartQty + currentPurchasedQty >= upgrade.maxPurchaseLimit)
        {
            Debug.Log($"Cannot add more {upgrade.name}: limit of {upgrade.maxPurchaseLimit} reached!");
            return;
        }

        // Increment the cart quantity
        cartQuantities[upgrade] = currentCartQty + 1;

        // If this is the first of this upgrade in the cart, instantiate a new prefab
        if (!cartItemObjects.ContainsKey(upgrade))
        {
            GameObject cartItemGO = Instantiate(upgradePrefab, cartPanel.transform);
            UpgradeItem cartItem = cartItemGO.GetComponent<UpgradeItem>();
            cartItem.upgrade = upgrade;

            // Disable drag-and-drop for cart items
            cartItem.enabled = false;

            // Update the text to show name, cost, and quantity
            TMPro.TMP_Text[] texts = cartItemGO.GetComponentsInChildren<TMPro.TMP_Text>();
            if (texts.Length > 0)
            {
                texts[0].text = $"{upgrade.name}";
            }
            if (texts.Length > 1)
            {
                texts[1].text = $"Cost: {upgrade.cost} (x{cartQuantities[upgrade]})";
            }

            cartItemObjects[upgrade] = cartItemGO;
        }
        else
        {
            // Update the existing cart item's text to reflect the new quantity
            GameObject cartItemGO = cartItemObjects[upgrade];
            TMPro.TMP_Text[] texts = cartItemGO.GetComponentsInChildren<TMPro.TMP_Text>();
            if (texts.Length > 1)
            {
                texts[1].text = $"Cost: {upgrade.cost} (x{cartQuantities[upgrade]})";
            }
        }
    }

    public void ClearCart()
    {
        // Destroy all cart item GameObjects
        foreach (var kvp in cartItemObjects)
        {
            Destroy(kvp.Value);
        }
        cartItemObjects.Clear();
        cartQuantities.Clear();
    }

    public void ConfirmPurchase()
    {
        int totalCost = 0;
        foreach (var kvp in cartQuantities)
        {
            totalCost += kvp.Key.cost * kvp.Value;
        }

        if (playerMoney >= totalCost)
        {
            playerMoney -= totalCost;
            UpdateMoneyDisplay();

            // Update purchased quantities
            foreach (var kvp in cartQuantities)
            {
                purchasedUpgrades[kvp.Key] = (purchasedUpgrades.ContainsKey(kvp.Key) ? purchasedUpgrades[kvp.Key] : 0) + kvp.Value;
            }

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