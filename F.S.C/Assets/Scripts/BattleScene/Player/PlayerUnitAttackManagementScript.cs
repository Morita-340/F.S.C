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
    public override void ChargeAttack(float time,float allTime,float chageTime,Vector3 TargetPosition){
        base.ChargeAttack(time,allTime,chageTime,TargetPosition);
    }
    /// <summary>
    /// 設定画面またはバトル開始時にチャージ武器を設定する
    /// </summary>
    public void SetChargeWeapon(WeaponBase ChargeWeapon){
        foreach(UnitData unitData in ChildrenUnitDatalist){
        Debug.LogWarning("KKK"+ unitData?.isPrime + ChildrenUnitDatalist.Count + unitData?.ReturnThisUnit().name);
            if(unitData.isPrime){
                if(unitData.ReturnThisUnit() is AttackUnit attackUnit){
                    attackUnit.SetChargeWeapon(ChargeWeapon);
                }
            }
        }
    }
}
