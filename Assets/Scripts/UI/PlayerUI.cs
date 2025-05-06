using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WallGraphicsGenerator;
using static TextureFunction;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
    public Player playerScript;
    public Camera mainCamera;
    public Image harpoonRing;
    public Image map;
    public Image mapBlur;
    public Image mapMask;
    public Image mapFrame;
    public Image oceanMap;
    public Image subMap;
    public Image harpTab;
    public Image harpTabBar;
    public Image inventory;
    public InventoryUI inventoryUI;
    public Slider hpBar;
    public Sprite tabSprite, tabBarSprite;
    public float charge;
    private Texture2D mapTexture, mapTextureB;
    private Vector2 mapPosOriginal;
    public bool seeingMap, tabMode = true, runningTabMode = false;
    public bool seeingInventory = false;
    public bool mouseOnUI = false;
    int UILayer;

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    void Start()
    {
        inventoryUI = inventory.GetComponent<InventoryUI>();
        UILayer = LayerMask.NameToLayer("UI");
        mapBlur = mapMask.transform.Find("BlurMap").GetComponent<Image>();
        seeingMap = false;
        tabSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi")[1];
        tabBarSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi_bars")[1];
        harpTab.sprite = tabSprite;
        harpTabBar.sprite = tabBarSprite;
        mapPosOriginal = mapMask.GetComponent<RectTransform>().anchoredPosition;
        mapTexture = GetTexture(publicMap);
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                if (mapTexture.GetPixel(x, y) == Color.black)
                {
                    mapTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        mapTexture.Apply();
        mapTexture.filterMode = FilterMode.Point;
        //mapTexture.Compress(false);
        map.sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 64);
        mapTextureB = GetTexture(publicMapTerrain);
        mapTextureB = GetWhiteAlpha(mapTextureB, publicGradientMap);
        mapTextureB.Apply();
        mapTextureB.filterMode = FilterMode.Point;
        //mapTextureB.Compress(false);
        mapBlur.sprite = Sprite.Create(mapTextureB, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 64);
        subMap = mapMask.transform.Find("SubMap").GetComponent<Image>();
        oceanMap = mapMask.transform.Find("OceanMap").GetComponent<Image>();
        subMap.color = new Color(0.1f, 0.3f, 0.65f, 1);
        oceanMap.color = new Color(0.1f, 0.3f, 0.65f, 0.6f);
    }
    void Update()
    {
        mouseOnUI = IsPointerOverUIElement();
        harpoonRing.gameObject.GetComponent<RectTransform>().localScale = Vector3.one / (mainCamera.orthographicSize / 6);
        harpoonRing.fillAmount = charge;
        if (Input.GetKeyDown(KeyCode.M) && !playerScript.onNPCUI)
        {
            mapMask.GetComponent<AudioSource>().Play();
            seeingMap = !seeingMap;
            seeingInventory = false;
            SetInventoryUI();
            SetMapUI();
        }
        if (Input.GetKeyDown(KeyCode.E) && !playerScript.onNPCUI)
        {
            seeingInventory = !seeingInventory;
            if (seeingMap)
                mapMask.GetComponent<AudioSource>().Play();
            seeingMap = false;
            SetMapUI();
            SetInventoryUI();
        }
        if (seeingMap)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                if (mapMask.GetComponent<RectTransform>().localScale.x >= (Vector3.one * 0.5f).x)
                    mapMask.GetComponent<RectTransform>().localScale -= Vector3.one * 0.05f;
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                if (mapMask.GetComponent<RectTransform>().localScale.x <= (Vector3.one * 5).x)
                    mapMask.GetComponent<RectTransform>().localScale += Vector3.one * 0.05f;
            }
        }
        else
        {
            mapMask.GetComponent<RectTransform>().localScale = Vector3.one * 0.77f;
        }
        if (Input.GetKeyDown(KeyCode.Tab) && !runningTabMode)
        {
            StartCoroutine(tabExtend());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            seeingMap = false;
            seeingInventory = false;

            SetMapUI();
            SetInventoryUI();
        }
        if (Input.anyKeyDown)
        {
            playerScript.onUI = seeingMap || seeingInventory;
        }
    }
    public void SetMapUI()
    {
        if (!seeingMap)
        {
            mapMask.GetComponent<Mask>().enabled = true;
            mapFrame.color = Color.white;
            mapMask.GetComponent<RectTransform>().anchoredPosition = mapPosOriginal;
            map.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 135);
            mapBlur.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 135);
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else
        {
            mapMask.GetComponent<Mask>().enabled = false;
            mapFrame.color = Color.clear;
            mapMask.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            map.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 400);
            mapBlur.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 400);
        }
    }
    public void SetInventoryUI()
    {
        if (seeingInventory)
        {
            inventory.enabled = true;
            inventoryUI.SetSlotsActive();
        }
        else
        {
            inventory.enabled = false;
            inventoryUI.SetSlotsActive();
        }
    }
    public IEnumerator tabExtend()
    {
        runningTabMode = true;
        tabMode = !tabMode;
        if (tabMode)
        {
            tabSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi")[1];
            tabBarSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi_bars")[1];
        }
        else
        {
            tabSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi")[0];
            tabBarSprite = Resources.LoadAll<Sprite>("Images/PlayerUI/At_Fi_bars")[0];
        }
        harpTab.sprite = tabSprite;
        harpTabBar.sprite = tabBarSprite;
        for (float i = 0; i <= 0.02f; i += 0.01f)
        {
            harpTabBar.fillAmount = i + 0.2f;
            harpTabBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-45 + (i * 110), harpTabBar.GetComponent<RectTransform>().anchoredPosition.y);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        harpTabBar.fillAmount = 1;
        for (float i = 1; i >= 0.04f; i -= 0.02f)
        {
            harpTabBar.fillAmount = i + 0.1f;
            harpTabBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-45 + (i * 110), harpTabBar.GetComponent<RectTransform>().anchoredPosition.y);
            yield return new WaitForSeconds(0.01f);
        }
        harpTabBar.fillAmount = 0;
        runningTabMode = false;
    }

}
