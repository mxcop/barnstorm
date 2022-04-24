using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTools : MonoBehaviour
{
    [HideInInspector] public Player plr;
    [HideInInspector] public Animator playerAnim;
    [HideInInspector] public bool isUsingTool;
    bool autoToolReuse;
    [SerializeField] Vector2[] offsets;
    [SerializeField] float lerpSpeed;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Space]
    [SerializeField] ContactFilter2D till_pushCf;
    [SerializeField] float till_pushRadius, till_pushForce;


    Vector3 targetPos;
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
                if (plr.container.FirstItemOfType(typeof(Hoe), 0, out switchTo))
                {
                    plr.HotbarSwitch(switchTo);
                    goto case ItemAction.Till;
                }
                break;

            case ItemAction.Till:
                playerAnim.SetBool("Tilling", true);
                lockedDirection = plr.animDir;
                isUsingTool = true;
                break;

            case ItemAction.Plant:
                ToolAction();
                autoToolReuse = true;
                break;
        }
        
        
    }

    private void LateUpdate()
    {
        PlayerAngle ang;
        if (isUsingTool) ang = lockedDirection;
        else ang = plr.animDir;
        Vector2Int v = GetPlayerOffsetPos(ang);

        targetPos = new Vector3(v.x+0.5f,v.y+0.5f,0);
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        if (autoToolReuse) ToolAction();
    }

    public void PlayerStopUse()
    {
        playerAnim.SetBool("Tilling", false);
        spriteRenderer.color = Color.white;

        autoToolReuse = false;
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

        if (plr.container.Peek(plr.slot, out it))
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
        if (!plr.isInBuilding)
        {
            Item item;
            if (GetHeldItem(out item))
            {
                switch (item.useAction)
                {
                    case ItemAction.Till:

                        if (CropManager.current != null)
                        {
                            Vector2Int pos = GetPlayerOffsetPos(lockedDirection);

                            // crop logic, tills only when on grass tiles, and not when hitting dirt
                            CropData? _crop = CropManager.current.Till(pos);
                            if (_crop != null)
                            {
                                CropData crop = (CropData)_crop;
                                int a = Random.Range((int)crop.amount.x, (int)crop.amount.y + 1);
                                for (int j = 0; j < a; j++)
                                {
                                    DroppedItem.DropOut(crop.item, 1, pos, Random.insideUnitCircle.normalized * 0.5f);
                                }
                            }
                        }
                        
                        // push players and enemies away when hitting them with the hoe
                        /*Collider2D[] colls = new Collider2D[3];
                        if (Physics2D.OverlapCircle(pos, till_pushRadius, till_pushCf, colls) > 0)
                        {
                            for(int i = 0; i< colls.Length; i++)
                            {
                                Collider2D c = colls[i];
                                if(c != null) c.GetComponent<Rigidbody2D>().AddForce((c.transform.position - plr.transform.position).normalized * till_pushForce);
                            }
                        }*/

                        break;

                    case ItemAction.Plant:
                        if (CropManager.current != null)
                        {
                            Vector2Int vec = GetPlayerOffsetPos(plr.animDir);
                            if (CropManager.current.TileIsOfType(TileType.Tilled, vec.x, vec.y))
                            {
                                if (CropManager.current.PlaceCrop(item.useAction_cropType, vec.x, vec.y))
                                {
                                    plr.container.RemoveItem(plr.slot, 1);
                                }
                            }
                        }
                        break;
                }
            }
        }

        isUsingTool = false;

    }

    public void Anim_ToolStart()
    {
        isUsingTool = true;
        spriteRenderer.color = Color.clear;
    }

}

public enum PlayerAngle
{
    Up = 0,
    Left = 1,
    Down = 2,
    Right = 3
}

