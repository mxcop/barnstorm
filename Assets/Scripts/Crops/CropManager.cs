using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public static CropManager current;
    [SerializeField] Tilemap grassMap, tilledMap, ambientMap;

    [SerializeField] CropPrefab[] _cropTypeLib;
    [SerializeField] TileBase grass, tilled;
    Dictionary<CropType, GameObject> cropTypeLib = new Dictionary<CropType, GameObject>();

    //crops auto-assign to this dictionary
    Dictionary<Vector3Int, Crop> crops = new Dictionary<Vector3Int, Crop>();

    private void Awake()
    {
        current = this;

        foreach (CropPrefab c in _cropTypeLib) { cropTypeLib.Add(c.type, c.prefab); }
    }

    public bool AddToCrops(Vector3Int pos, Crop crop)
    {
        if (GetFromCrops(pos) != null) return false;
        else
        {
            crops.Add(pos, crop);
            return true;
        }
    }

    public Crop GetFromCrops(Vector3Int pos)
    {
        if (crops.ContainsKey(pos))
        {
            if (crops[pos] != null)
            {
                return crops[pos];
            }
            else
            {
                crops.Remove(pos);
                return null;
            }
        }
        else return null;
    }

    public bool PlaceCrop(CropType type, int x, int y)
    {
        if (GetFromCrops(new Vector3Int(x,y,0)) == null)
        {
            Instantiate(cropTypeLib[type], new Vector3(x, y, 0), Quaternion.identity);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Places a random tile of the given type, doesn't work on indestructibles
    /// </summary>
    /// <param name="type"></param>
    public void PlaceTile(TileType type, int x, int y)
    {
        switch (type)
        {
            case TileType.Tillable:
                grassMap.SetTile(new Vector3Int(x, y, 0), grass);
                tilledMap.SetTile(new Vector3Int(x, y, 0), null);
                break;

            case TileType.Tilled:
                tilledMap.SetTile(new Vector3Int(x, y, 0), tilled);
                break;
        }
    }

    public bool TileIsOfType(TileType type, int x, int y)
    {
        switch (type)
        {
            default:
                return false;

            case TileType.Tillable:
                return (grassMap.GetTile(new Vector3Int(x, y, 0)) != null && ambientMap.GetTile(new Vector3Int(x, y, 0)) == null);

            case TileType.Tilled:
                Debug.Log((tilledMap.GetTile(new Vector3Int(x, y, 0)) != null));
                return (tilledMap.GetTile(new Vector3Int(x, y, 0)) != null);
        }
        
    }

    public CropData? Till(Vector2Int pos) => Till(pos.x,pos.y);
    public CropData? Till(int x, int y)
    {
        if (TileIsOfType(TileType.Tillable, x, y))
        {
            PlaceTile(TileType.Tilled, x, y);
        }

        return GetFromCrops(new Vector3Int(x, y, 0))?.Harvest();
    }

}

public enum TileType
{
    Tillable,
    Tilled
}

[System.Serializable]
public struct CropPrefab
{
    public CropType type;
    public GameObject prefab;
}

public enum CropType
{
    Potato,
    Carrot,
    Corn,
    Pumpkin,
    Pepper
}
