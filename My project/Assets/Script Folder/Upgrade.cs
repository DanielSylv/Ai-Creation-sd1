using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public string name;
    public int cost;
    public string category;
    public Sprite icon;
    public int maxPurchaseLimit = 3;
}
