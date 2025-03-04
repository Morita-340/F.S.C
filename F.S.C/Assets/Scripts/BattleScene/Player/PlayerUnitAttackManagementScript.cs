using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitAttackManagementScript : AbstractUnitAttackManagementScript
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    public override void NormalAttack(float time,Vector3 TargetPosition){
        base.NormalAttack(time,TargetPosition);
    }
    public override void ChargeAttack(float inputTime,float processSpan,float chargeTime,Vector3 TargetPosition){
        base.ChargeAttack(inputTime,processSpan,chargeTime,TargetPosition);
    }
    /// <summary>
    /// 設定画面またはバトル開始時にチャージ武器を設定する
    /// </summary>
    public void SetChargeWeapon(WeaponBase ChargeWeapon){
        foreach(UnitData unitData in ChildrenUnitDatalist){
            if(unitData.isPrime){
                if(unitData.ReturnThisUnit() is AttackUnit attackUnit){
                    attackUnit.SetChargeWeapon(ChargeWeapon);
                }
            }
        }
    }
}
