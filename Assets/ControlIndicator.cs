using System.Collections.Generic;
using UnityEngine;


public class ControlIndicator : MonoBehaviour
{
    [SerializeField] private ButtonProfile button;
    [SerializeField] private float showRadius = 2.0f;
    [SerializeField] private Vector2 offset;

    private bool showingHint = false;
    private GameObject indicator;

    private Interactable interactable;

    private static ContactFilter2D filter;

    private void Awake()
    {
        if (filter.useLayerMask == false)
        {
            filter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Player")
            };
        }

        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {
        CheckForPlayers();
    }

    /// <summary>
    /// Change the state of the indicator. (Whether it is showing or not)
    /// </summary>
    /// <param name="state">The state to change to.</param>
    /// <param name="player">The player who is changing it.</param>
    public void ChangeState(bool state, Player player)
    {        

        if (showingHint == false && state == true && PersistentPlayerManager.main.TryGetPlayer(player.playerID, out PersistentPlayer p))
        {
            Sprite sprite = button switch
            {
                ButtonProfile.North => p.controlsProfile.North,
                ButtonProfile.East => p.controlsProfile.East,
                ButtonProfile.South => p.controlsProfile.South,
                ButtonProfile.West => p.controlsProfile.West,
                _ => null
            };

            indicator = IndicatorManager.Spawn(sprite, (Vector2)transform.position + offset);
        }

        if ((showingHint == true && state == false) || (state == false && indicator != null && LeanTween.isTweening(indicator) == false))
        {
            IndicatorManager.Despawn(indicator);
        }

        showingHint = state;
    }

    /// <summary>
    /// Check if there are player close.
    /// </summary>
    private void CheckForPlayers()
    {
        if (interactable != null && interactable.inUse)
        {
            ChangeState(false, null);
            return;
        }

        List<Collider2D> res = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, showRadius, filter, res);

        (Player, float) closest = (null, Mathf.Infinity);
        for (int i = 0; i < res.Count; i++)
        {
            if (res[i] != null)
            {
                Player c = res[i].gameObject.GetComponent<Player>();
                if (c != null)
                {
                    float d = Vector2.Distance(c.transform.position, transform.position);
                    if (closest.Item2 > d) closest = (c, d);
                }
            }
        }

        if (closest.Item1 != null)
            ChangeState(true, closest.Item1);
        else
            ChangeState(false, null);
    }
}
