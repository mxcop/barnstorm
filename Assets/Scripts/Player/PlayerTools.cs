using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTools : MonoBehaviour
{
    [HideInInspector] public Player plr;
    [HideInInspector] public Animator playerAnim;
    [HideInInspector] public bool isUsing;
    [SerializeField] Vector2[] offsets;

    PlayerAngle lockedDirection;
    
    /// <summary>
    /// Called by the player using the interact key
    /// </summary>
    public void PlayerUse()
    {
        Item item;
        ItemAction act = ItemAction.None;

        if (GetHeldItem(out item)) act = item.useAction;

        switch (act)
        {
            case ItemAction.None:
                int switchTo;
                if (plr.container.FirstItemOfType(typeof(Hoe), out switchTo))
                {
                    plr.HotbarSwitch(switchTo);
                    goto case ItemAction.Till;
                }
                break;

            case ItemAction.Till:
                playerAnim.SetBool("Tilling", true);
                lockedDirection = plr.animDir;
                isUsing = true;
                break;

            case ItemAction.Plant:
                ToolAction();
                break;
        }
        
    }

    public void PlayerStopUse()
    {
        playerAnim.SetBool("Tilling", false);
    }

    Vector2Int GetPlayerOffsetPos(PlayerAngle dir)
    {
        Vector2 v = (Vector2)plr.transform.position + offsets[(int)dir];
        return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    }

    bool GetHeldItem(out Item it)
    {
        //if (plr.container.ContainsAt(typeof(Hoe), plr.selected)) return ItemAction.Till;
        //else return ItemAction.None;

        if (plr.container.Peek(plr.selected, out it))
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Called by the player animator, uses the currently selected tool's function
    /// </summary>
    /// <param name="selectedItem"></param>
    /// <param name="dir"></param>
    public void ToolAction()
    {
        Item item;
        if (GetHeldItem(out item))
        {
            switch (item.useAction)
            {
                case ItemAction.Till:

                    CropData? _crop = CropManager.current.Till(GetPlayerOffsetPos(lockedDirection));
                    if (_crop != null)
                    {
                        CropData crop = (CropData)_crop;
                        for (int j = 0; j < crop.amount; j++)
                        {
                            DroppedItem.DropOut(crop.item, 1, GetPlayerOffsetPos(lockedDirection), Random.insideUnitCircle.normalized * 0.5f);
                        }
                    }
                    break;

                case ItemAction.Plant:
                    Vector2Int vec = GetPlayerOffsetPos(plr.animDir);
                    if (CropManager.current.TileIsOfType(TileType.Tilled, vec.x, vec.y))
                    {
                        if (CropManager.current.PlaceCrop((item as Food).cropType, vec.x, vec.y))
                        {
                            plr.container.RemoveItem(plr.selected, 1);
                        }
                    }
                    break;
            }
        }

        isUsing = false;

    }

    public void Anim_ToolStart() => isUsing = true;

}

public enum PlayerAngle
{
    Up = 0,
    Left = 1,
    Down = 2,
    Right = 3
}

