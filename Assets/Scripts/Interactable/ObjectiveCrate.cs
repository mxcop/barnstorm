using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;

public class ObjectiveCrate : Crate
{
    protected override void OnUpdate(int _, ContainedItem<Item> item) {
        base.OnUpdate(_, item);
        StageManager.current.UpdateObjective(item);
    }
}
