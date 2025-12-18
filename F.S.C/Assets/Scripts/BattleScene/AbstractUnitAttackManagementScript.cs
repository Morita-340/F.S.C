using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractUnitAttackManagementScript : MonoBehaviour
{
    [SerializeField]
    private AbstractUnitDestroyManagementScript AUDMS;
    [SerializeField,Range(0.1f,10f)]
    private float normalAttackInterval = 1;
    [SerializeField,Range(0.1f,10f)]
    protected float chargeAttackInterval =1;
    [SerializeField]protected List<UnitData> ChildrenUnitDatalist;
    bool inTimeRangeOFNormalAttack = true;
    float NextNormalAttack = 0f;
    bool inTimeRangeOFChargeAttack = false;
    float nextChargeAttackTime = 0f;
    [SerializeField,ReadOnly]protected Vector3 TargetPosition;
    public void SetTargetPosition(Vector3 inputTargetPosion){
        TargetPosition = inputTargetPosion;
        foreach (UnitData child in ChildrenUnitDatalist){
            //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
            if(child.ReturnThisUnit() is AttackUnit AU){
                //AU.SetTargetPosition(TargetPosition);
            }
        }
    }
    protected virtual void Awake(){
    }
    protected virtual void Start(){
        ChildrenUnitDatalist = AUDMS.GetChildUnitDataList();
    }
    /// <summary>
    /// 呼び出されたら一定時間おきに各ユニットの通常攻撃を"毎フレームではなく一度だけ"実行する
    /// </summary>
    /// <param name="time">ロックオンし始めてから経過した時間。ロックオンが外れると0に戻る</param>
    public virtual void NormalAttack(float time){
        if(time < 0){NextNormalAttack = 0;}
        if(time >= NextNormalAttack){
            Debug.Log("AUAMS normal time" + time +" "+ NextNormalAttack);
            foreach (UnitData child in ChildrenUnitDatalist){
                //Debug.Log("AUAMS normal unit" + child.ReturnThisUnit().name);
                if(child.ReturnThisUnit() is AttackUnit AU){
                    //StartCoroutine(AU.NormalAttack(TargetPosition));
                }
            }
            NextNormalAttack += normalAttackInterval;
        }
    }
    /// <summary>
    /// 呼び出されたら一定時間おきに各ユニットのチャージ攻撃を実行する
    /// </summary>
    /// <param name="time"></param>
    public virtual void ChargeAttack(float inputTime,float processSpan,float chargeTime){
        if(inputTime % processSpan > chargeTime){
            if(!inTimeRangeOFChargeAttack){
                inTimeRangeOFChargeAttack = true;
                nextChargeAttackTime =inputTime;}
            if(inputTime >= nextChargeAttackTime){
                foreach (UnitData child in ChildrenUnitDatalist){
                    if(child.ReturnThisUnit() is AttackUnit AU){
                        AU.ChargeAttack(TargetPosition);
                    }
                }
                nextChargeAttackTime += chargeAttackInterval;
            }
        }else{inTimeRangeOFChargeAttack = false;}
    }
    public void ChargeAttack(){
        foreach (UnitData child in ChildrenUnitDatalist){
            if(child.ReturnThisUnit() is AttackUnit AU){
                AU.ChargeAttack(TargetPosition);
            }
        }
    }
}
