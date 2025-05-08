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
    public override void NormalAttack(float time){
        base.NormalAttack(time);
    }
    public override void ChargeAttack(float inputTime,float processSpan,float chargeTime){
        base.ChargeAttack(inputTime,processSpan,chargeTime);
    }
    /// <summary>
    /// 設定画面またはバトル開始時にチャージ武器を設定する
    /// </summary>
    public void SetChargeWeapon(WeaponBase ChargeWeapon){
        foreach(UnitData unitData in ChildrenUnitDatalist){
            if(unitData == null){continue;}
            if(unitData.isPrime){
                if(unitData.ReturnThisUnit() is CoreBase coreBase){
                    coreBase.SetChargeWeapon(ChargeWeapon);
                }
            }
        }
    }
}
