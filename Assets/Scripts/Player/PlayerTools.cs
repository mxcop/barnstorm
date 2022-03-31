using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTools : MonoBehaviour
{
    [HideInInspector] public Player plr;
    [HideInInspector] public Animator playerAnim;
    [HideInInspector] public bool isUsing;
    [SerializeField] Vector2[] offsets;

    ItemAction currentAction;
    PlayerAngle lockedDirection;
    
    /// <summary>
    /// Called by the player using the interact key
    /// </summary>
    public void PlayerUse()
    {
        isUsing = true;

        switch (GetHeldItemAction())
        {
            case ItemAction.Till:
                playerAnim.SetBool("Tilling", true);
                lockedDirection = plr.animDir;
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

    ItemAction GetHeldItemAction()
    {
        if (plr.container.ContainsAt(typeof(Hoe), plr.selected)) return ItemAction.Till;
        else return ItemAction.None;
    }

    /// <summary>
    /// Called by the player animator, uses the currently selected tool's function
    /// </summary>
    /// <param name="selectedItem"></param>
    /// <param name="dir"></param>
    public void Anim_ToolAction()
    {
        isUsing = false;

        switch (GetHeldItemAction())
        {
            case ItemAction.Till:               

                CropData? _crop = CropManager.current.Till(GetPlayerOffsetPos(lockedDirection));
                if (_crop != null)
                {
                    CropData crop = (CropData)_crop;
                    plr.container.PushItem(crop.item, crop.amount);
                }
                break;
        }
        
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

enum ItemAction
{
    None,
    Plant,
    Till
}