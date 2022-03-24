using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public static CropManager current;
    [SerializeField] Tilemap tilemap;

    [SerializeField] CropData[] grass, tilled, crops;
    Dictionary<string, CropData> dict = new Dictionary<string, CropData>();

    private void Awake()
    {
        current = this;

        foreach (CropData c in grass) { dict.Add(c.name, c); }
        foreach (CropData c in tilled) { dict.Add(c.name, c); }
        foreach (CropData c in crops) { dict.Add(c.name, c); }
    }

    /// <summary>
    /// Places a random tile of the given type, doesn't work on indestructibles
    /// </summary>
    /// <param name="type"></param>
    public void PlaceTile(TileType type, int x, int y)
    {
        Tile t = null;
        switch (type)
        {
            case TileType.Grass:
                t = grass[Random.Range(0, grass.Length)].tile;
                break;

            case TileType.Tilled:
                t = tilled[Random.Range(0, tilled.Length)].tile;
                break;

                //food crops
            case TileType.Potato:
                t = GetCropData("potato0").tile;
                break;

            case TileType.Carrot:
                t = GetCropData("carrot0").tile;
                break;

            case TileType.Corn:
                t = GetCropData("corn0").tile;
                break;

            case TileType.Pumpkin:
                t = GetCropData("pumpkin0").tile;
                break;

            case TileType.Pepper:
                t = GetCropData("pepper0").tile;
                break;
        }

        if (t != null)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), t);
        }
    }

    public CropData GetCropData(string n) => dict[n];

    public bool TryGetTile(int x, int y, out CropDataTile tileData)
    {
        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
        if (tile != null && dict.ContainsKey(tile.name))
        {
            tileData = new CropDataTile(dict[tile.name], x, y);
            return true;
        }
        else
        {
            tileData = new CropDataTile();
            return false;
        }
    }

}

[System.Serializable]
public struct CropData
{
    public Tile tile;
    public string name => tile.name;
    public TileType type;

    [Space]
    public Item item;
    public int yield;
}

public struct CropDataTile
{
    public int x, y;
    public CropData cropData;

    public CropDataTile(CropData _cropData, int _x, int _y)
    {
        cropData = _cropData;
        x = _x;
        y = _y;
    }
}

public enum TileType
{
    Indestructible,
    Grass,
    Tilled,
    Potato,
    Carrot,
    Corn,
    Pumpkin,
    Pepper
}
