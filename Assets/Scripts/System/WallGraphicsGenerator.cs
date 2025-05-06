using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using static TextureFunction;
using static MathBr;

public class WallGraphicsGenerator : MonoBehaviour
{
    [Header("Config", order = 0)]
    public int expandWhite = 9;
    public int blurInBlack = 15;
    public int blur = 15;
    public static int expandWhiteNum;
    public static int blurInBlackNum;
    public static int blurNum;
    [Header("Essentials", order = 1)]
    public Texture2D map;
    public Texture2D terrainMap;
    public Texture2D expandedWhiteAreaMap;
    public Texture2D blurredMap;
    public Texture2D spawnMap;
    public Texture2D palmeirasMap;
    public Texture2D colorsTex;
    public Texture2D whiteMap;
    public int chunkSize = 16;
    public GameObject Ocean, player, boat;
    public Grid<FloorTile> tileMap;
    public List<Tile> tiles;
    public List<Sprite> alphaList;
    public GameObject[,] floors;
    public GameObject[,] subFloors;
    private Texture2D blankTex;
    private Sprite blankSprite;
    public static Texture2D publicMap, publicWhiteMap, publicGradientMap, publicMapTerrain;
    public static Color oceanSinkColor;

    /// <summary>
    /// GameObject Principal de todas as cenas.
    /// </summary>
    public static GameObject GAME;

    public List<GameObject> worldChunks;

    //[Header("Trees")]
    public Sprite[] trees;

    public void Start()
    {
        GAME = transform.parent.gameObject;

        expandWhiteNum = expandWhite;
        blurInBlackNum = blurInBlack;
        blurNum = blur;

        publicMapTerrain = GetTexture(terrainMap);
        publicMap = GetTexture(map);

        worldChunks = new List<GameObject>();
        palmeirasMap = new Texture2D(map.width, map.height);
        publicWhiteMap = new Texture2D(map.width, map.height);
        publicGradientMap = new Texture2D(map.width, map.height);

        GenerateNoiseTexture(1000, 0.45f, 1f, 0.35f, palmeirasMap);
        publicWhiteMap = GetTexture(map);
        publicWhiteMap = PolarizedGray(publicWhiteMap, 0.1f);
        whiteMap = GetTexture(publicWhiteMap);
        expandedWhiteAreaMap = GetTexture(publicWhiteMap);
        ExpandWhiteArea(expandedWhiteAreaMap, expandWhite);
        blurredMap = ApplyBlurOnlyInBlack(expandedWhiteAreaMap, blurInBlack);
        blurredMap = ApplyBlur(expandedWhiteAreaMap, blur);

        publicGradientMap = GetTexture(blurredMap);
        //GeneratePngTilesAlpha(sand[0].texture);
        for (int o = 0; o < tiles.Count; o++)
        {
            tiles[o].color = colorsTex.GetPixel(o, 0);
        }
        Ocean.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
        Ocean.GetComponent<SpriteRenderer>().size = new Vector2(200, 200);
        blankTex = new Texture2D(16, 16);

        for (int x = 0; x < spawnMap.width; x++)
        {
            for (int y = 0; y < spawnMap.height; y++)
            {
                if (spawnMap.GetPixel(x, y).r > 0 && spawnMap.GetPixel(x, y).g == 0 && spawnMap.GetPixel(x, y).b == 0)
                {
                    Ocean.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    player.transform.position = new Vector3(x + 0.5f, y + 0.5f, player.transform.position.z);
                }
                if (spawnMap.GetPixel(x, y).r == 0 && spawnMap.GetPixel(x, y).g > 0 && spawnMap.GetPixel(x, y).b == 0)
                    boat.transform.position = new Vector3(x + 0.5f, y + 0.5f, player.transform.position.z);
            }
        }
        for (int x = 0; x < blankTex.width; x++)
        {
            for (int y = 0; y < blankTex.height; y++)
            {
                blankTex.SetPixel(x, y, Color.clear);
            }
        }
        blankTex.Apply();
        blankSprite = Sprite.Create(blankTex, new Rect(0, 0, blankTex.width, blankTex.height), new Vector2(0.5f, 0.5f));
        blankSprite.name = "blankTile";
        tileMap = new Grid<FloorTile>(map.width, map.height, (Grid<FloorTile> g, int x, int y) => new FloorTile(g, x, y, false));
        floors = new GameObject[map.width, map.height];
        subFloors = new GameObject[map.width, map.height];
        CreateChunks();
        GenerateMap(map);
        GenerateTreesInMap(map);
    }
    public void CreateChunks()
    {
        int numChunks = map.width / chunkSize;
        int i = 0;
        for (int y = 0; y < numChunks; y++)
        {
            for (int x = 0; x < numChunks; x++)
            {
                GameObject newChunk = new GameObject();
                newChunk.name = ("x" + x.ToString() + "y" + y.ToString());
                newChunk.transform.position = new Vector3(x * chunkSize, y * chunkSize, 0);
                newChunk.transform.parent = transform;
                worldChunks.Add(newChunk);
                i++;
            }
        }
    }
    public void GenerateNoiseTexture(float seed, float frequency, float limit, float scattering, Texture2D noiseTexture)
    {
        //noiseTexture = new Texture2D(mapWidth, mapHeight);
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v + UnityEngine.Random.Range(-scattering, scattering) > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }
        noiseTexture.Apply();
    }
    public void GenerateTreesInMap(Texture2D mip)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                if (map.GetPixel(x, y) == colorsTex.GetPixel(1, 0) && palmeirasMap.GetPixel(x, y) == Color.white)
                {
                    var currentTile = new GameObject("Tree");
                    var s = UnityEngine.Random.Range(1f, 2.5f);
                    currentTile.transform.localScale = new Vector3(s, s, s);
                    currentTile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    DefineChunkParent(currentTile);
                    currentTile.layer = 6;
                    currentTile.AddComponent<SpriteRenderer>();
                    currentTile.GetComponent<SpriteRenderer>().sprite = trees[UnityEngine.Random.Range(0, trees.Length)];
                    currentTile.GetComponent<SpriteRenderer>().sortingLayerName = "Moveable";
                    currentTile.GetComponent<SpriteRenderer>().sortingOrder = (int)-(currentTile.transform.position.y * 10);
                    s = UnityEngine.Random.Range(1, 10);
                    Flip(currentTile, s > 5 ? true : false);
                }
            }
        }
    }
    public void GenerateMap(Texture2D mip)
    {
        GameObject currentTile;
        for (int w = 0; w < tiles.Count; w++)
        {
            var map = PreserveOneColor(mip, tiles[w].color);
            for (int xx = 0; xx < map.width; xx++)
            {
                for (int yy = 0; yy < map.height; yy++)
                {
                    map.SetPixel(xx, yy, new Color(map.GetPixel(xx, yy).r, map.GetPixel(xx, yy).g, map.GetPixel(xx, yy).b, 1));
                    if (map.GetPixel(xx, yy) == Color.white)
                    {
                        tileMap.GetGridObject(xx, yy).active = true;
                    }
                    else
                        tileMap.GetGridObject(xx, yy).active = false;
                }
            }
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    
                    if (map.GetPixel(x, y) == Color.white)
                    {
                        floors[x, y] = new GameObject(tiles[w].name + "Floor");
                        currentTile = floors[x, y];
                        currentTile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                        currentTile.layer = 6;
                        currentTile.AddComponent<SpriteRenderer>();
                        currentTile.GetComponent<SpriteRenderer>().sprite = tiles[w].image;
                        currentTile.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        currentTile.GetComponent<SpriteRenderer>().sortingOrder = tiles[w].order;
                        currentTile.AddComponent<BoxCollider2D>();
                        currentTile.transform.parent = gameObject.transform;
                        DefineChunkParent(currentTile);
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (i + x >= map.width || x + i < 0 || j + y >= map.height || y + j < 0)
                                    continue;
                                if (map.GetPixel(x + i, y + j) == Color.black && (tileMap.GetGridObject(x + i, y + j).active == false))
                                {
                                    tileMap.GetGridObject(x + i, y + j).active = true;
                                    floors[x + i, y + j] = new GameObject(tiles[w].name + "Border");
                                    floors[x + i, y + j].transform.position = new Vector3(x + i + 0.5f, y + j + 0.5f, 0);
                                    floors[x + i, y + j].AddComponent<SpriteRenderer>();
                                    floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = blankSprite;
                                    floors[x + i, y + j].GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                                    floors[x + i, y + j].GetComponent<SpriteRenderer>().sortingOrder = tiles[w].order;
                                    floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite.name = "blankTile";
                                    floors[x + i, y + j].transform.parent = gameObject.transform;
                                    DefineChunkParent(floors[x + i, y + j]);
                                }
                            }
                        }
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (CheckIfInsideArrayLimits(floors, x + i, y + j))
                                    if (map.GetPixel(x + i, y + j) == Color.black && floors[x + i, y + j] != null)
                                    {
                                        if (!Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//4
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[17]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//3sup
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[15]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//3esq
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[14]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//3dir
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[13]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//3inf
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[16]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//2
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[9]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//2
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[10]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//2
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[11]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//2
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[12]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//1sup
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[1]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//1esq
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[3]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//1dir
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[5]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//1inf
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[7]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");

                                        if (Sup(x + i, y + j, map) && !Esq(x + i, y + j, map) && !Dir(x + i, y + j, map) && Inf(x + i, y + j, map))//2hori
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[3]), PassBorderAlpha(tiles[w].image, alphaList[5]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                        if (!Sup(x + i, y + j, map) && Esq(x + i, y + j, map) && Dir(x + i, y + j, map) && !Inf(x + i, y + j, map))//2vert
                                            floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[1]), PassBorderAlpha(tiles[w].image, alphaList[7]), floors[x + i, y + j].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                                    }
                            }
                        }

                        //esqsup
                        if (CheckIfInsideArrayLimits(floors, x - 1, y + 1) && floors[x - 1, y + 1] != null)
                            if (EsqSup(x, y, map))
                                floors[x - 1, y + 1].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[0]), floors[x - 1, y + 1].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");

                        //dirsup
                        if (CheckIfInsideArrayLimits(floors, x + 1, y + 1) && floors[x + 1, y + 1] != null)
                            if (DirSup(x, y, map))
                                floors[x + 1, y + 1].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[2]), floors[x + 1, y + 1].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");

                        //esqinf
                        if (CheckIfInsideArrayLimits(floors, x - 1, y - 1) && floors[x - 1, y - 1] != null)
                            if (EsqInf(x, y, map))
                                floors[x - 1, y - 1].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[6]), floors[x - 1, y - 1].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");

                        //dirinf
                        if (CheckIfInsideArrayLimits(floors, x + 1, y - 1) && floors[x + 1, y - 1] != null)
                            if (DirInf(x, y, map))
                                floors[x + 1, y - 1].GetComponent<SpriteRenderer>().sprite = MergeSprites(16, 16, new Sprite[] { PassBorderAlpha(tiles[w].image, alphaList[8]), floors[x + 1, y - 1].GetComponent<SpriteRenderer>().sprite }, "sandBorderSprite");
                    }
                    else if (blurredMap.GetPixel(x, y).r > 0 && w == 0)
                    {
                        subFloors[x, y] = new GameObject("SubSand");
                        subFloors[x, y].transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                        subFloors[x, y].AddComponent<SpriteRenderer>();
                        subFloors[x, y].GetComponent<SpriteRenderer>().sprite = tiles[0].image;
                        subFloors[x, y].GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                        subFloors[x, y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, blurredMap.GetPixel(x, y).r);
                        subFloors[x, y].transform.parent = gameObject.transform;
                        DefineChunkParent(subFloors[x, y]);
                    }
                    else
                        continue;
                }
            }
        }
    }
    public void DefineChunkParent(GameObject obj)
    {
        int chunkCoordX = Mathf.RoundToInt(((obj.transform.position.x / chunkSize) * chunkSize) - 0.5f);
        chunkCoordX /= chunkSize;
        int chunkCoordY = Mathf.RoundToInt(((obj.transform.position.y / chunkSize) * chunkSize) - 0.5f);
        chunkCoordY /= chunkSize;
        //print(chunkCoordX + "-" + chunkCoordY);
        if (chunkCoordX < worldChunks.Count && chunkCoordY < worldChunks.Count)
        {
            for(int i = 0; i < worldChunks.Count; i++)
            {
                if (worldChunks[i].name == "x" + chunkCoordX + ("y" + chunkCoordY))
                {
                    obj.transform.parent = worldChunks[i].transform;
                    break;
                }
            }
        }
    }
    bool EsqSup(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x - 1, y + 1) == Color.black;
    bool Sup(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x, y + 1) == Color.black;
    bool DirSup(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x + 1, y + 1) == Color.black;
    bool Esq(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x - 1, y) == Color.black;
    bool Dir(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x + 1, y) == Color.black;
    bool EsqInf(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x - 1, y - 1) == Color.black;
    bool Inf(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x, y - 1) == Color.black;    
    bool DirInf(int x, int y, Texture2D borderTexture) => borderTexture.GetPixel(x + 1, y - 1) == Color.black;
    public static bool CheckArrayLimits(GameObject[,,] floors, int ex, int ey) => ex >= floors.GetLength(0) || ey >= floors.GetLength(1) || ex < 0 || ey < 0;
    public static bool CheckIfInsideArrayLimits(GameObject[,] floors, int ex, int ey) => ex >= 0 && ey >= 0 && ex < floors.GetLength(0) && ey < floors.GetLength(1);
    public static bool CheckIfInsideArrayLimits(Texture2D map, int ex, int ey) => ex >= 0 && ey >= 0 && ex < map.width && ey < map.height;

    public void GenerateNoiseTexture()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {

            }
        }
    }

    private void Flip(GameObject obj, bool t)
    {
        if (t == true)
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            obj.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
public class FloorTile
{
    public int x, y;
    public Grid<FloorTile> grid { get; private set; }
    public List<Sprite> spriteList;
    public bool active;

    public FloorTile (Grid<FloorTile> grid, int x, int y, bool a)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        active = a;
    }
}