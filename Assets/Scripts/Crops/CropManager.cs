using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public static CropManager current;
    [SerializeField] Tilemap grassMap, tilledMap, cropsMap, ambientMap;

    [SerializeField] CropData[] crops;
    [SerializeField] CropData grass, tilled;
    Dictionary<string, CropData> dict = new Dictionary<string, CropData>();

    private void Awake()
    {
        current = this;

        foreach (CropData c in crops) { dict.Add(c.name, c); }
    }

    /// <summary>
    /// Places a random tile of the given type, doesn't work on indestructibles
    /// </summary>
    /// <param name="type"></param>
    public void PlaceTile(TileType type, int x, int y)
    {
        switch (type)
        {
            case TileType.Grass:
                grassMap.SetTile(new Vector3Int(x, y, 0), grass.tile);
                tilledMap.SetTile(new Vector3Int(x, y, 0), null);
                break;

            case TileType.Tilled:
                tilledMap.SetTile(new Vector3Int(x, y, 0), tilled.tile);
                break;

                //food crops
            case TileType.Potato:
                cropsMap.SetTile(new Vector3Int(x, y, 0), GetCropData("potato0").tile);
                break;

            case TileType.Carrot:
                cropsMap.SetTile(new Vector3Int(x, y, 0), GetCropData("carrot0").tile);
                break;

            case TileType.Corn:
                cropsMap.SetTile(new Vector3Int(x, y, 0), GetCropData("corn0").tile);
                break;

            case TileType.Pumpkin:
                cropsMap.SetTile(new Vector3Int(x, y, 0), GetCropData("pumpkin0").tile);
                break;

            case TileType.Pepper:
                cropsMap.SetTile(new Vector3Int(x, y, 0), GetCropData("pepper0").tile);
                break;
        }
    }

    public CropData GetCropData(string n) => dict[n];

    public bool TileIsTillable(int x, int y)
    {
        if (grassMap.GetTile(new Vector3Int(x, y, 0)) != null && ambientMap.GetTile(new Vector3Int(x, y, 0)) == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public CropDataTile? Till(Vector2 pos) => Till(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    public CropDataTile? Till(int x, int y)
    {
        if (TileIsTillable(x, y))
        {
            PlaceTile(TileType.Tilled, x, y);
        }

        TileBase t = cropsMap.GetTile(new Vector3Int(x, y, 0));
        if (t != null)
        {
            cropsMap.SetTile(new Vector3Int(x, y, 0), null);
            return new CropDataTile(GetCropData(t.name), x, y);
        }
        else return null;
    }

}

[System.Serializable]
public struct CropData
{
    public TileBase tile;
    public string name => tile.name;
    public TileType type;

    [Space]
    public Item item;
    public int yield;
}

public class CropDataTile
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
