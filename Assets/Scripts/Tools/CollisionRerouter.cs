using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRerouter : MonoBehaviour
{
    [SerializeField] GameObject _collisionTarget, _triggerTarget;
    ICollisionRerouting collisionTarget;
    ITriggerRerouting triggerTarget;

    private void Awake()
    {
        if(_collisionTarget != null)collisionTarget = _collisionTarget.GetComponent<ICollisionRerouting>();
        if(_triggerTarget != null) triggerTarget = _triggerTarget.GetComponent<ITriggerRerouting>();
    }

    private void OnCollisionEnter2D(Collision2D collision) => collisionTarget?.ReCollisionEnter(collision);
    private void OnCollisionStay2D(Collision2D collision) => collisionTarget?.ReCollisionStay(collision);
    private void OnCollisionExit2D(Collision2D collision) => collisionTarget?.ReCollisionExit(collision);

    private void OnTriggerEnter2D(Collider2D collision) => triggerTarget?.ReTriggerEnter(collision);
    private void OnTriggerStay2D(Collider2D collision) => triggerTarget?.ReTriggerStay(collision);
    private void OnTriggerExit2D(Collider2D collision) => triggerTarget?.ReTriggerExit(collision);
}

public interface ICollisionRerouting
{
    public void ReCollisionEnter(Collision2D coll);
    public void ReCollisionStay(Collision2D coll);
    public void ReCollisionExit(Collision2D coll);
}

public interface ITriggerRerouting
{
    public void ReTriggerEnter(Collider2D coll);
    public void ReTriggerStay(Collider2D coll);
    public void ReTriggerExit(Collider2D coll);
}
