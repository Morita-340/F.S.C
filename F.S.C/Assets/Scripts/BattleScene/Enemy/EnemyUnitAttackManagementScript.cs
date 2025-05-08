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
    public override void NormalAttack(float time){
        base.NormalAttack(time);
    }
    public override void ChargeAttack(float time,float allTime,float chageTime){
        base.ChargeAttack(time,allTime,chageTime);
    }
}
