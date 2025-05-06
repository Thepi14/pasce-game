using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name = "NULL";
    public Sprite icon;
    public int value;
    public int amount;
    public int maxAmount;

    public bool isHarpoon;

    public Item()
    {
        name = "NULL";
        icon = Resources.Load("Images/PlayerUI/clear") as Sprite;
        this.isHarpoon = false;
    }
    public Item(string name)
    {
        this.name = name;
        this.isHarpoon = false;
    }
    public Item(string name, Sprite icon, int value, int maxAmount)
    {
        this.name = name;
        this.icon = icon;
        this.value = value;
        this.maxAmount = maxAmount;
        this.isHarpoon = false;
    }
    public Item(string name, Sprite icon, int value, int maxAmount, int amount)
    {
        this.name = name;
        this.icon = icon;
        this.value = value;
        this.maxAmount = maxAmount;
        this.amount = amount;
        this.isHarpoon = false;
    }
    public Item(string name, Sprite icon, int value, int amount, int maxAmount, bool isHarpoon) : this(name, icon, value, amount, maxAmount)
    {
        this.isHarpoon = isHarpoon;
    }

    /// <summary>
    /// Função que faz o objeto ser vazio
    /// </summary>
    public void SetEmptyItem()
    {
        name = "NULL";
        icon = Resources.Load("Images/PlayerUI/clear") as Sprite;
        value = 0;
        maxAmount = 0;
        amount = 0;
        isHarpoon = false;
    }

    public void TurnIntoItem(Item item)
    {
        name = item.name;
        icon = item.icon;
        value = item.value;
        maxAmount = item.maxAmount;
        amount = item.amount;
        isHarpoon = item.isHarpoon;
    }
}
