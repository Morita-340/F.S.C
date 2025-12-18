using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefinePlayerUnitAttackManagementScript : RefineAbstractUnitAttackManagementScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    public override bool SetTargetPosition(Vector3 inputTargetPosion,RaycastHit2D enemyHit2D)
    {
        return base.SetTargetPosition(inputTargetPosion,enemyHit2D);
    }
}
