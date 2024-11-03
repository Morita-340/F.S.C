using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitAttackManagementScript : AbstractUnitAttackManagementScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    public override void NormalAttack(float time,Vector3 TargetPosition){
        base.NormalAttack(time,TargetPosition);
    }
    public override void ChargeAttack(float time,float allTime,float chageTime,Vector3 TargetPosition){
        base.ChargeAttack(time,allTime,chageTime,TargetPosition);
    }
}
