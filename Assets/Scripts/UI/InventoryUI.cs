using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public PlayerUI playerUI;
    public Player player;

    public GameObject moneyTab;
    public TextMeshProUGUI moneyAmount;
    public int money = 0;
    public GameObject[] slots;
    public Item[] items;

    public GameObject playerImage;
    public GameObject cardImage;

    public GameObject PickedItemImage;

    public Item harpoonItem;
    public GameObject harpoonSlot;

    public Item originalItemSlot;

    public bool pickedItem = false;
    public Item itemPicked;

    public int x, y;

    //public const int MAX_AMOUNT = 16;

    private void OnValidate()
    {

    }

    void Start()
    {
        PickedItemImage = transform.Find("PickedItemImage").gameObject;
        playerUI = transform.parent.GetComponent<PlayerUI>();
        player = playerUI.transform.parent.Find("Player").GetComponent<Player>();
        moneyTab = transform.parent.Find("MoneyTab").gameObject;
        moneyAmount = moneyTab.transform.Find("Amount").gameObject.GetComponent<TextMeshProUGUI>();
        slots = new GameObject[10];
        items = new Item[10];

        harpoonItem = new Item();
        harpoonSlot = transform.Find("harpoonSlot").gameObject;
        harpoonSlot.GetComponent<Button>().onClick.AddListener(delegate { TickPickedItem("harpoonSlot"); });

        for (int i = 0; i < transform.childCount; i++)
        {
            if (/*int.Parse(transform.GetChild(i).name) == i*/ transform.GetChild(i).name == i.ToString())
            {
                slots[i] = transform.GetChild(i).gameObject;
                slots[i].GetComponent<Button>().onClick.AddListener(delegate { TickPickedItem(i); });
            }
            else
                continue;

            items[i] = new Item();
            items[i].SetEmptyItem();
        }
        UpdateInventorySlots();
    }
    public void TickPickedItem(int slot)
    {
        if (!pickedItem)
        {
            pickedItem = true;
            itemPicked = items[slot];
            items[slot].SetEmptyItem();

            originalItemSlot = items[slot];
        }
        else if (pickedItem && items[slot].name == "NULL")
        {
            pickedItem = false;
            items[slot].TurnIntoItem(itemPicked);
            itemPicked.SetEmptyItem();
        }
        else if (pickedItem && items[slot].name != "NULL")
        {
            Item c = new Item();
            c.TurnIntoItem(itemPicked);
            itemPicked.TurnIntoItem(items[slot]);
            items[slot].TurnIntoItem(c);
        }
    }
    public void TickPickedItem(string slotName)
    {
        if (!pickedItem && slotName == "harpoonSlot")
        {
            originalItemSlot = harpoonItem;
            itemPicked = harpoonItem;
        }
        else if (pickedItem && slotName == "harpoonSlot" && !itemPicked.isHarpoon && harpoonItem.name == "NULL")
        {

        }
        else if (pickedItem && slotName == "harpoonSlot" && itemPicked.isHarpoon && harpoonItem.name == "NULL")
        {
            harpoonItem.TurnIntoItem(itemPicked);
            itemPicked.SetEmptyItem();
        }
    }


    void Update()
    {
        moneyAmount.text = money.ToString();
        PickedItemImage.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition - new Vector3(x, y, 0);
    }
    public bool AddItem(Item item)
    {
        Item pseudoItem = new Item(item.name, item.icon, item.value, item.maxAmount, item.amount);
        bool hasThisItemInInventory = false;
        bool canPutThisItem = false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (items[i].name == pseudoItem.name)
            {
                if (items[i].amount + pseudoItem.amount <= pseudoItem.maxAmount)
                {
                    items[i].amount += pseudoItem.amount;
                    hasThisItemInInventory = true;
                    canPutThisItem = true;
                    pseudoItem.amount = 0;
                    break;
                }
                else if (items[i].amount + pseudoItem.amount > pseudoItem.maxAmount)
                {
                    items[i].amount = pseudoItem.maxAmount;
                    pseudoItem.amount = (items[i].amount + pseudoItem.amount) - pseudoItem.maxAmount;
                }
            }
        }
        if (!hasThisItemInInventory)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (items[i].name == "NULL" && pseudoItem.amount > 0)
                {
                    canPutThisItem = true;
                    if (pseudoItem.amount >= pseudoItem.maxAmount)
                    {
                        items[i] = new Item(pseudoItem.name, pseudoItem.icon, pseudoItem.value, pseudoItem.maxAmount);
                        pseudoItem.amount -= pseudoItem.maxAmount;
                    }
                    else if (pseudoItem.amount < pseudoItem.maxAmount && pseudoItem.amount > 0)
                    {
                        items[i] = new Item(pseudoItem.name, pseudoItem.icon, pseudoItem.value, pseudoItem.amount);
                        items[i].amount = pseudoItem.amount;
                        pseudoItem.amount = 0;
                    }
                    //break;
                }
                else if (pseudoItem.amount == 0)
                    break;
            }
        }
        UpdateInventorySlots();
        item.amount = pseudoItem.amount;
        return canPutThisItem;
    }
    public Item GetItems (string itemName, int amount)
    {
        int pseudoAmount = amount + 0;
        Item returnItem = new Item(itemName);
        List<Item> emptyItemList = new List<Item>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (items[i].name == itemName)
            {
                returnItem.TurnIntoItem(items[i]);
                returnItem.amount = amount;
                if (items[i].amount - pseudoAmount == 0)
                {
                    items[i].SetEmptyItem();
                    for (int j = 0; j < emptyItemList.Count; j++)
                    {
                        emptyItemList[j].SetEmptyItem();
                    }
                    UpdateInventorySlots();
                    return returnItem;
                }
                else if (items[i].amount - pseudoAmount > 0)
                {
                    items[i].amount -= pseudoAmount;
                    for (int j = 0; j < emptyItemList.Count; j++)
                    {
                        emptyItemList[j].SetEmptyItem();
                    }
                    UpdateInventorySlots();
                    return returnItem;
                }
                else if (items[i].amount - pseudoAmount < 0)
                {
                    pseudoAmount = pseudoAmount - items[i].amount;
                    emptyItemList.Add(items[i]);
                    //items[i].SetEmptyItem();
                }
            }
        }
        return null;
    }
    public Item SellItems(string itemName, int amount)
    {
        int pseudoAmount = amount + 0;
        Item returnItem = new Item(itemName);
        List<Item> emptyItemList = new List<Item>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (items[i].name == itemName)
            {
                returnItem.TurnIntoItem(items[i]);
                returnItem.amount = amount;
                if (items[i].amount - pseudoAmount == 0)
                {
                    items[i].SetEmptyItem();
                    for (int j = 0; j < emptyItemList.Count; j++)
                    {
                        emptyItemList[j].SetEmptyItem();
                    }
                    UpdateInventorySlots();

                    money += returnItem.value * amount;

                    return returnItem;
                }
                else if (items[i].amount - pseudoAmount > 0)
                {
                    items[i].amount -= pseudoAmount;
                    for (int j = 0; j < emptyItemList.Count; j++)
                    {
                        emptyItemList[j].SetEmptyItem();
                    }
                    UpdateInventorySlots();

                    money += returnItem.value * amount;

                    return returnItem;
                }
                else if (items[i].amount - pseudoAmount < 0)
                {
                    pseudoAmount = pseudoAmount - items[i].amount;
                    emptyItemList.Add(items[i]);
                    //items[i].SetEmptyItem();
                }
            }
        }
        return null;
    }
    public void UpdateInventorySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = items[i].amount.ToString();
            if (items[i].name != "NULL")
            {
                slots[i].GetComponent<Image>().sprite = items[i].icon;
                if (items[i].amount > 1 && GetComponent<Image>().enabled)
                {
                    slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                else
                {
                    slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.clear;
                }
            }
            else
            {
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.clear;
            }
        }
    } 
    public void SetSlotsActive()
    {
        harpoonSlot.GetComponent<Image>().enabled = GetComponent<Image>().enabled;
        harpoonSlot.GetComponent<Button>().enabled = GetComponent<Image>().enabled;

        playerImage.GetComponent<Image>().enabled = GetComponent<Image>().enabled;
        cardImage.GetComponent<Image>().enabled = GetComponent<Image>().enabled;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponent<Image>().enabled = GetComponent<Image>().enabled;
            slots[i].GetComponent<Button>().enabled = GetComponent<Image>().enabled;
            if (GetComponent<Image>().enabled && items[i].amount > 1)
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.clear;
        }
    }
    public void SetSlotsActive(bool a)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponent<Image>().enabled = a;
            if (a && items[i].amount > 1)
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                slots[i].transform.Find("Amount").GetComponent<TextMeshProUGUI>().color = Color.clear;
        }
    }
}
