using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Device Profile Sprites", order = 1)]
public class DeviceProfileSprites : ScriptableObject
{
    [Header("Actions")]
    public Sprite North;
    public Sprite East;
    public Sprite South;
    public Sprite West;
    [Space(10)]
    public Sprite CompactNorth;
    public Sprite CompactEast;
    public Sprite CompactSouth;
    public Sprite CompactWest;
}
