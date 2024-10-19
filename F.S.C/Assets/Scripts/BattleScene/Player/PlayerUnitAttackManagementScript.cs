using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitAttackManagementScript : MonoBehaviour
{
    [SerializeField]
    private PlayerUnitDestroyManagementScript PUDMS;
    [SerializeField,Range(0f,10f)]
    private float normalAttackInterval = 1;
    [SerializeField,Range(0f,10f)]
    private float chargeAttackInterval =1;
    private List<UnitData> ChildrenUnitDatalist;
    bool inTimeRangeOFNormalAttack = true;
    float NextNormalAttack = 0f;
    bool inTimeRangeOFChargeAttack = false;
    float NextChargeAttack = 0f;
    void Start(){
        ChildrenUnitDatalist = PUDMS.GetChildUnitDataList();
    }
    /// <summary>
    /// 呼び出されたら一定時間おきに各ユニットの通常攻撃を"毎フレームではなく一度だけ"実行する
    /// </summary>
    /// <param name="time">ロックオンし始めてから経過した時間。ロックオンが外れると0に戻る</param>
    public void NormalAttack(float time,Vector3 TargetPosition){
        if(time == 0){NextNormalAttack = 0;}
        if(time >= NextNormalAttack){
            foreach (UnitData child in ChildrenUnitDatalist){
                child.ReturnThisUnit().NormalAttack(TargetPosition);
            }
            NextNormalAttack += normalAttackInterval;
        }
    }
    /// <summary>
    /// 呼び出されたら一定時間おきに各ユニットのチャージ攻撃を実行する
    /// </summary>
    /// <param name="time"></param>
    public void ChargeAttack(float time,float allTime,float chageTime,Vector3 TargetPosition){
        if(time % allTime > chageTime){
            if(!inTimeRangeOFChargeAttack){
                inTimeRangeOFChargeAttack = true;
                NextChargeAttack =time;}
            if(time >= NextChargeAttack){
        Debug.Log("MCCS PUAMS charge" + time +" "+ NextChargeAttack);
                foreach (UnitData child in ChildrenUnitDatalist){
                    child.ReturnThisUnit().ChargeAttack(TargetPosition);
                }
                NextChargeAttack += chargeAttackInterval;
            }
        }else{inTimeRangeOFChargeAttack = false;}
    }
}
