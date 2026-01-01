using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineEnemyUnitAttackManagementScript : RefineAbstractUnitAttackManagementScript
{
    protected override void Start()
    {
        base.Start();
    }
    public override bool SetTargetPosition(Vector3 inputTargetPosion, RaycastHit2D enemyHit2D)
    {
        //Debug.LogAssertion("NNNNNNN");
        return base.SetTargetPosition(inputTargetPosion, enemyHit2D);
    }
}
